using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using _build.Extensions;
using _build.Helpers;
using _build.Models;
using CliWrap;
using FluentFTP;
using Microsoft.IdentityModel.Tokens;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Renci.SshNet;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace _build;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main()
    {
        return Execute<Build>(x => x.Publish);
    }

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly string FtpPassword;
    [Parameter] readonly string SshPassword;
    [Parameter] readonly string FtpHost;
    [Parameter] readonly string FtpUser;
    [Parameter] readonly string SshHost;
    [Parameter] readonly string SshUser;
    [Parameter] readonly string ServerHost;
    [Parameter] readonly string JwtKey;
    [Parameter] readonly string JwtIssuer;
    [Parameter] readonly string JwtAudience;
    [Parameter] readonly string AndroidSigningKeyPass;
    [Parameter] readonly string AndroidSigningStorePass;
    static readonly Dictionary<string, string> Hosts = new();
    static readonly Dictionary<Project, ServiceOptions> ServiceOptions = new();
    static readonly List<Project> ServiceProjects = new();
    static string Token;
    static DirectoryInfo AndroidFolder;

    const FtpListOption FtpOption =
        FtpListOption.Recursive | FtpListOption.ForceList | FtpListOption.Auto | FtpListOption.AllFiles;

    [Solution] readonly Solution Solution;

    Target Setup =>
        _ => _
            .Executes(() =>
                {
                    Token = CreteToken();
                    ServiceProjects.AddRange(Solution.GetProjects("Service"));
                    ushort port = 5000;

                    foreach (var serviceProject in ServiceProjects)
                    {
                        ServiceOptions[serviceProject] = new ServiceOptions(port, serviceProject.Name);
                        Hosts[serviceProject.GetOptionsName()] = $"https://{ServerHost}:{port}";
                        port++;
                    }
                }
            );

    Target SetupAppSettings =>
        _ => _
            .DependsOn(Setup)
            .Executes(() =>
                {
                    foreach (var project in Solution.AllProjects)
                    {
                        project.SetGetAppSettingsFile(Token, ServiceOptions, Hosts, ServerHost);
                    }
                }
            );

    Target Clean =>
        _ => _
            .DependsOn(SetupAppSettings)
            .Executes(() => DotNetClean(setting => setting.SetProject(Solution)));

    Target Restore =>
        _ => _
            .DependsOn(Clean)
            .Executes(() => DotNetRestore(setting => setting.SetProjectFile(Solution)));

    Target Compile =>
        _ => _
            .DependsOn(Restore)
            .Executes(() =>
                {
                    foreach (var project in Solution.AllProjects.Where(x =>
                                 !x.Name.Contains("Android") && x.Name != "Spravy.Ui.Desktop"
                             ))
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            try
                            {
                                DotNetBuild(setting =>
                                    setting.SetProjectFile(project)
                                        .EnableNoRestore()
                                        .SetConfiguration(Configuration)
                                );

                                break;
                            }
                            catch (Exception e)
                            {
                                if (i == 2)
                                {
                                    throw;
                                }

                                if (e.ToString().Contains("CompileAvaloniaXamlTask"))
                                {
                                    continue;
                                }

                                throw;
                            }
                        }
                    }

                    var desktop = Solution.AllProjects.Single(x => x.Name == "Spravy.Ui.Desktop").ThrowIfNull();

                    for (var i = 0; i < 3; i++)
                    {
                        try
                        {
                            DotNetBuild(setting =>
                                setting.SetProjectFile(desktop)
                                    .EnableNoRestore()
                                    .SetRuntime("linux-x64")
                                    .SetConfiguration(Configuration)
                            );

                            break;
                        }
                        catch (Exception e)
                        {
                            if (i == 2)
                            {
                                throw;
                            }

                            if (e.ToString().Contains("CompileAvaloniaXamlTask"))
                            {
                                continue;
                            }

                            throw;
                        }
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        try
                        {
                            DotNetBuild(setting =>
                                setting.SetProjectFile(desktop)
                                    .EnableNoRestore()
                                    .SetRuntime("win-x64")
                                    .SetConfiguration(Configuration)
                            );

                            break;
                        }
                        catch (Exception e)
                        {
                            if (i == 2)
                            {
                                throw;
                            }

                            if (e.ToString().Contains("CompileAvaloniaXamlTask"))
                            {
                                continue;
                            }

                            throw;
                        }
                    }
                }
            );

    Target PublishServices =>
        _ => _
            .DependsOn(Compile)
            .Executes(() =>
                {
                    using var sshClient = new SshClient(CreateSshConnection());
                    sshClient.Connect();
                    using var ftpClient = CreateFtpClient();
                    ftpClient.Connect();

                    foreach (var serviceOption in ServiceOptions)
                    {
                        serviceOption.Key.DeployService(
                            sshClient,
                            ftpClient,
                            PathHelper.PublishFolder,
                            Configuration,
                            FtpUser,
                            SshPassword
                        );
                    }

                    foreach (var serviceProject in ServiceProjects)
                    {
                        var serviceName = serviceProject.Name.ToLower();
                        sshClient.SafeRun($"echo {SshPassword} | sudo -S systemctl enable {serviceName}");
                        sshClient.SafeRun($"echo {SshPassword} | sudo -S systemctl restart {serviceName}");
                    }

                    sshClient.SafeRun($"echo {SshPassword} | sudo -S systemctl daemon-reload");
                }
            );

    Target PublishDesktop =>
        _ => _
            .DependsOn(PublishServices)
            .Executes(() =>
                {
                    DeployDesktop("linux-x64");
                    DeployDesktop("win-x64");
                }
            );

    Target PublishBrowser =>
        _ => _
            .DependsOn(Compile)
            .Executes(() =>
                {
                    using var sshClient = new SshClient(CreateSshConnection());
                    sshClient.Connect();
                    using var ftpClient = CreateFtpClient();
                    ftpClient.Connect();
                    var browserProject = Solution.AllProjects.Single(x => x.Name == "Spravy.Ui.Browser").ThrowIfNull();
                    var appBundlePath = "bin/Release/net7.0/browser-wasm/AppBundle";
                    var appBundleFolder = Path.Combine(browserProject.Directory, appBundlePath).ToFolder();
                    ftpClient.DeleteIfExistsFolder($"/home/{FtpUser}/{browserProject.Name}".ToFolder());
                    ftpClient.UploadDirectory(appBundleFolder.FullName, $"/home/{FtpUser}/{browserProject.Name}");
                    sshClient.SafeRun($"echo {SshPassword} | sudo -S rm -rf /var/www/spravy.com.ua/html/*");
                    sshClient.SafeRun(
                        $"echo {SshPassword} | sudo -S cp -rf /home/{FtpUser}/{browserProject.Name}/* /var/www/spravy.com.ua/html"
                    );
                    sshClient.SafeRun($"echo {SshPassword} | sudo -S chown -R $USER:$USER /var/www/spravy.com.ua/html");
                    sshClient.SafeRun($"echo {SshPassword} | sudo -S chmod -R 755 /var/www/spravy.com.ua");
                    sshClient.SafeRun($"echo {SshPassword} | sudo -S systemctl restart nginx");
                    sshClient.SafeRun($"echo {SshPassword} | sudo -S systemctl reload nginx");
                }
            );

    Target PublishAndroid =>
        _ => _
            .DependsOn(PublishServices)
            .Executes(() =>
                {
                    using var ftpClient = CreateFtpClient();
                    ftpClient.Connect();
                    var keyStoreFile = new FileInfo("/tmp/Spravy/sign-key.keystore");

                    if (keyStoreFile.Directory is null)
                    {
                        throw new NullReferenceException();
                    }

                    keyStoreFile.Directory.CreateIfNotExits();

                    if (!keyStoreFile.Exists)
                    {
                        Cli.Wrap("keytool")
                            .WithArguments(new[]
                                {
                                    "-genkey",
                                    "-v",
                                    "-keystore",
                                    keyStoreFile.FullName,
                                    "-alias",
                                    "spravy",
                                    "-keyalg",
                                    "RSA",
                                    "-keysize",
                                    "2048",
                                    "-validity",
                                    "10000",
                                    "-dname",
                                    "CN=Serhii Maksymov, OU=Serhii Maksymov FOP, O=Serhii Maksymov FOP, L=Kharkiv, S=Kharkiv State, C=Ukraine",
                                    "-storepass",
                                    AndroidSigningStorePass,
                                }
                            )
                            .RunCommand();
                    }

                    var android = Solution.AllProjects.Single(x => x.Name == "Spravy.Ui.Android");

                    AndroidFolder = android.PublishProject(PathHelper.PublishFolder, Configuration, setting =>
                        setting
                            .SetProperty("AndroidKeyStore", "true")
                            .SetProperty("AndroidSigningKeyStore", keyStoreFile.FullName)
                            .SetProperty("AndroidSigningKeyAlias", "spravy")
                            .SetProperty("AndroidSigningKeyPass", AndroidSigningKeyPass)
                            .SetProperty("AndroidSigningStorePass", AndroidSigningStorePass)
                            .SetProperty("AndroidSdkDirectory", "/usr/lib/android-sdk")
                            .DisableNoBuild()
                    );

                    DeleteIfExistsDirectory(ftpClient, $"/home/{FtpUser}/Apps/Spravy.Ui.Android");
                    CreateIfNotExistsDirectory(ftpClient, $"/home/{FtpUser}/Apps");
                    ftpClient.UploadDirectory(AndroidFolder.FullName, $"/home/{FtpUser}/Apps/Spravy.Ui.Android");
                }
            );

    Target Publish => _ => _.DependsOn(PublishDesktop, PublishAndroid, PublishBrowser);

    void DeployDesktop(string runtime)
    {
        using var ftpClient = CreateFtpClient();
        ftpClient.Connect();
        var desktop = Solution.AllProjects.Single(x => x.Name == "Spravy.Ui.Desktop").ThrowIfNull();
        var desktopFolder = desktop.PublishProject(PathHelper.PublishFolder.Combine(runtime), Configuration,
            settings => settings.SetRuntime(runtime).SetOutput("WinExe")
        );
        ftpClient.DeleteIfExistsFolder($"/home/{FtpUser}/Apps/Spravy.Ui.Desktop".ToFolder().Combine(runtime));
        ftpClient.CreateIfNotExistsDirectory($"/home/{FtpUser}/Apps".ToFolder().Combine(runtime));
        ftpClient.UploadDirectory(desktopFolder.FullName, $"/home/{FtpUser}/Apps/Spravy.Ui.Desktop/{runtime}");
    }

    static void CreateIfNotExistsDirectory(FtpClient client, string path)
    {
        if (client.DirectoryExists(path))
        {
            return;
        }

        client.CreateDirectory(path, true);
    }

    static void DeleteIfExistsDirectory(FtpClient client, string path)
    {
        if (!client.DirectoryExists(path))
        {
            return;
        }

        try
        {
            client.DeleteDirectory(path, FtpOption);
        }
        catch
        {
            Log.Error("{Path}", path);

            throw;
        }
    }

    static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        var dir = new DirectoryInfo(sourceDir);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
        }

        var dirs = dir.GetDirectories();
        Directory.CreateDirectory(destinationDir);

        foreach (var file in dir.GetFiles())
        {
            var targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        if (!recursive)
        {
            return;
        }

        foreach (var subDir in dirs)
        {
            var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir, true);
        }
    }

    string CreteToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var expires = DateTime.UtcNow.AddDays(30);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "Service"),
        };

        var token = new JwtSecurityToken(
            JwtIssuer,
            JwtAudience,
            claims,
            expires: expires,
            signingCredentials: signingCredentials
        );

        var jwt = jwtSecurityTokenHandler.WriteToken(token);

        return jwt;
    }

    ConnectionInfo CreateSshConnection()
    {
        var values = SshHost.Split(":");
        var password = new PasswordAuthenticationMethod(SshUser, SshPassword);

        if (values.Length == 2)
        {
            return new ConnectionInfo(values[0], int.Parse(values[1]), SshUser, password);
        }

        return new ConnectionInfo(values[0], SshUser, password);
    }

    FtpClient CreateFtpClient()
    {
        var values = FtpHost.Split(":");

        if (values.Length == 2)
        {
            return new FtpClient(values[0], FtpUser, FtpPassword, int.Parse(values[1]));
        }

        return new FtpClient(FtpHost, FtpUser, FtpPassword);
    }
}
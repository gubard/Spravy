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
using Telegram.Bot;
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
        LoadVersion();
        Version++;
        UpdateVersion();

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
    [Parameter] readonly string TelegramToken;
    [Parameter] readonly string StagingFtpPassword;
    [Parameter] readonly string StagingSshPassword;
    [Parameter] readonly string StagingFtpHost;
    [Parameter] readonly string StagingFtpUser;
    [Parameter] readonly string StagingSshHost;
    [Parameter] readonly string StagingSshUser;
    [Parameter] readonly string StagingServerHost;
    [Parameter] readonly string MailPassword;
    static SpravyVersion Version;

    static readonly Dictionary<string, string> Hosts = new();
    static readonly Dictionary<Project, ServiceOptions> ServiceOptions = new();
    static readonly List<Project> ServiceProjects = new();
    static string Token;
    static DirectoryInfo AndroidFolder;

    const FtpListOption FtpOption =
        FtpListOption.Recursive | FtpListOption.ForceList | FtpListOption.Auto | FtpListOption.AllFiles;

    [Solution] readonly Solution Solution;

    static readonly FileInfo FileVersion = "/tmp/Spravy/version.txt".ToFile();

    static void LoadVersion()
    {
        if (FileVersion.Exists)
        {
            SpravyVersion.TryParse(File.ReadAllText(FileVersion.FullName), out Version);

            return;
        }

        Version = new SpravyVersion(1, 0, 0, 0);
    }

    static void UpdateVersion()
    {
        if (!FileVersion.Directory.Exists)
        {
            FileVersion.Directory.Create();
        }

        if (!FileVersion.Exists)
        {
            using var stream = FileVersion.Create();
        }

        File.WriteAllText(FileVersion.FullName, Version.ToString());
    }

    void Setup(string host)
    {
        Token = CreteToken();
        ServiceProjects.Clear();
        ServiceProjects.AddRange(Solution.GetProjects("Service"));
        ushort port = 5000;

        foreach (var serviceProject in ServiceProjects)
        {
            ServiceOptions[serviceProject] = new ServiceOptions(port, serviceProject.Name);
            Hosts[serviceProject.GetOptionsName()] = $"https://{host}:{port}";
            port++;
        }
    }

    void SetupAppSettings(string domain)
    {
        foreach (var project in Solution.AllProjects)
        {
            project.SetGetAppSettingsFile(Token, ServiceOptions, Hosts, domain, MailPassword);
        }
    }

    void Clean()
    {
        DotNetClean(setting => setting.SetProject(Solution).SetConfiguration(Configuration));
    }

    void Restore()
    {
        DotNetRestore(setting => setting.SetProjectFile(Solution));
    }

    void Compile()
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
                            .AddProperty("Version", Version.ToString())
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
                        .AddProperty("Version", Version.ToString())
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
                        .AddProperty("Version", Version.ToString())
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

    void PublishServices(
        string ftpHost,
        string ftpUser,
        string ftpPassword,
        string sshHost,
        string sshUser,
        string sshPassword
    )
    {
        using var sshClient = new SshClient(CreateSshConnection(sshHost, sshUser, sshPassword));
        sshClient.Connect();
        using var ftpClient = CreateFtpClient(ftpHost, ftpUser, ftpPassword);
        ftpClient.Connect();

        foreach (var serviceOption in ServiceOptions)
        {
            serviceOption.Key.DeployService(
                sshClient,
                ftpClient,
                PathHelper.PublishFolder,
                Configuration,
                ftpUser,
                sshPassword
            );
        }

        foreach (var serviceProject in ServiceProjects)
        {
            var serviceName = serviceProject.Name.ToLower();
            sshClient.SafeRun($"echo {sshPassword} | sudo -S systemctl enable {serviceName}");
            sshClient.SafeRun($"echo {sshPassword} | sudo -S systemctl restart {serviceName}");
        }

        sshClient.SafeRun($"echo {sshPassword} | sudo -S systemctl daemon-reload");
    }

    void PublishDesktop(string ftpHost, string ftpUser, string ftpPassword)
    {
        DeployDesktop("linux-x64", ftpHost, ftpUser, ftpPassword);
        DeployDesktop("win-x64", ftpHost, ftpUser, ftpPassword);
    }

    void PublishAndroid(string ftpHost, string ftpUser, string ftpPassword)
    {
        using var ftpClient = CreateFtpClient(ftpHost, ftpUser, ftpPassword);
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
                .SetProperty("AndroidSdkDirectory", "/opt/android-sdk")
                .DisableNoBuild()
                .AddProperty("Version", Version.ToString())
        );

        DeleteIfExistsDirectory(ftpClient, $"/home/{ftpUser}/Apps/Spravy.Ui.Android");
        CreateIfNotExistsDirectory(ftpClient, $"/home/{ftpUser}/Apps");
        ftpClient.UploadDirectory(AndroidFolder.FullName, $"/home/{ftpUser}/Apps/Spravy.Ui.Android");
    }

    void PublishBrowser(
        string ftpHost,
        string ftpUser,
        string ftpPassword,
        string sshHost,
        string sshUser,
        string sshPassword
    )
    {
        using var sshClient = new SshClient(CreateSshConnection(ftpHost, ftpUser, ftpPassword));
        sshClient.Connect();
        using var ftpClient = CreateFtpClient(sshHost, sshUser, sshPassword);
        ftpClient.Connect();
        var browserProject = Solution.AllProjects.Single(x => x.Name == "Spravy.Ui.Browser").ThrowIfNull();
        var appBundlePath = "bin/Release/net8.0/browser-wasm/AppBundle";
        var appBundleFolder = Path.Combine(browserProject.Directory, appBundlePath).ToFolder();
        ftpClient.DeleteIfExistsFolder($"/home/{ftpUser}/{browserProject.Name}".ToFolder());
        ftpClient.UploadDirectory(appBundleFolder.FullName, $"/home/{ftpUser}/{browserProject.Name}");
        sshClient.SafeRun($"echo {sshPassword} | sudo -S rm -rf /var/www/spravy.com.ua/html/*");
        sshClient.SafeRun(
            $"echo {sshPassword} | sudo -S cp -rf /home/{ftpUser}/{browserProject.Name}/* /var/www/spravy.com.ua/html"
        );
        sshClient.SafeRun(
            $"echo {sshPassword} | sudo -S cp -rf /home/{ftpUser}/Apps/Spravy.Ui.Android/com.SerhiiMaksymovFOP.Spravy-Signed.apk /var/www/spravy.com.ua/html"
        );
        sshClient.SafeRun(
            $"echo {sshPassword} | sudo -S cp -rf /home/{ftpUser}/Apps/Spravy.Ui.Android/com.SerhiiMaksymovFOP.Spravy-Signed.aab /var/www/spravy.com.ua/html"
        );
        sshClient.SafeRun(
            $"cd /home/vafnir/Apps/Spravy.Ui.Desktop/linux-x64 && echo {sshPassword} | zip -r /var/www/spravy.com.ua/html/Spravy.Linux-x64.zip ./*"
        );
        sshClient.SafeRun(
            $"cd /home/vafnir/Apps/Spravy.Ui.Desktop/win-x64 && echo {sshPassword} | zip -r /var/www/spravy.com.ua/html/Spravy.Windows-x64.zip ./*"
        );
        sshClient.SafeRun($"echo {sshPassword} | sudo -S chown -R $USER:$USER /var/www/spravy.com.ua/html");
        sshClient.SafeRun($"echo {sshPassword} | sudo -S chmod -R 755 /var/www/spravy.com.ua");
        sshClient.SafeRun($"echo {sshPassword} | sudo -S systemctl restart nginx");
        sshClient.SafeRun($"echo {sshPassword} | sudo -S systemctl reload nginx");
    }

    Target StagingSetup => _ => _.Executes(() => Setup(StagingServerHost));
    Target ProdSetup => _ => _.DependsOn(StagingPublishBrowser).Executes(() => Setup(ServerHost));

    Target StagingSetupAppSettings =>
        _ => _.DependsOn(StagingSetup).Executes(() => SetupAppSettings(StagingServerHost));

    Target ProdSetupAppSettings => _ => _.DependsOn(ProdSetup).Executes(() => SetupAppSettings(ServerHost));

    Target StagingClean => _ => _.DependsOn(StagingSetupAppSettings).Executes(Clean);
    Target ProdClean => _ => _.DependsOn(ProdSetupAppSettings).Executes(Clean);

    Target StagingRestore => _ => _.DependsOn(StagingClean).Executes(Restore);
    Target ProdRestore => _ => _.DependsOn(ProdClean).Executes(Restore);

    Target StagingCompile => _ => _.DependsOn(StagingRestore).Executes(Compile);
    Target ProdCompile => _ => _.DependsOn(ProdRestore).Executes(Compile);

    Target StagingPublishServices =>
        _ => _.DependsOn(CleanStagingDataBase, StagingCompile)
            .Executes(() => PublishServices(
                    StagingFtpHost,
                    StagingFtpUser,
                    StagingFtpPassword,
                    StagingSshHost,
                    StagingSshUser,
                    StagingSshPassword
                )
            );

    Target CleanStagingDataBase =>
        _ => _.Executes(() =>
            {
                using var client =
                    new SshClient(CreateSshConnection(StagingSshHost, StagingSshUser, StagingSshPassword));
                client.Connect();
                client.SafeRun($"echo {StagingSshPassword} | sudo -S rm -fr /home/{StagingFtpUser}/DataBases");
            }
        );

    Target Test =>
        _ => _.DependsOn(StagingPublishServices)
            .Executes(() => DotNetTest(s =>
                    s.SetConfiguration(Configuration)
                        .EnableNoRestore()
                        .EnableNoBuild()
                        .SetProjectFile(Solution.AllProjects.Single(x => x.Name.EndsWith(".Tests")))
                )
            );

    Target ProdPublishServices =>
        _ => _.DependsOn(ProdCompile)
            .Executes(() => PublishServices(FtpHost, FtpUser, FtpPassword, SshHost, SshUser, SshPassword));

    Target StagingPublishDesktop =>
        _ => _.DependsOn(Test)
            .Executes(() => PublishDesktop(StagingFtpHost, StagingFtpUser, StagingFtpPassword));

    Target ProdPublishDesktop =>
        _ => _.DependsOn(ProdPublishServices).Executes(() => PublishDesktop(FtpHost, FtpUser, FtpPassword));

    Target StagingPublishAndroid =>
        _ => _.DependsOn(Test)
            .Executes(() => PublishAndroid(StagingFtpHost, StagingFtpUser, StagingFtpPassword));

    Target ProdPublishAndroid =>
        _ => _.DependsOn(ProdPublishServices).Executes(() => PublishAndroid(FtpHost, FtpUser, FtpPassword));

    Target StagingPublishBrowser =>
        _ => _
            .DependsOn(StagingPublishAndroid, StagingPublishDesktop)
            .Executes(() => PublishBrowser(
                    StagingFtpHost,
                    StagingFtpUser,
                    StagingFtpPassword,
                    StagingSshHost,
                    StagingSshUser,
                    StagingSshPassword
                )
            );

    Target ProdPublishBrowser =>
        _ => _
            .DependsOn(ProdPublishAndroid, ProdPublishDesktop)
            .Executes(() => PublishBrowser(
                    FtpHost,
                    FtpUser,
                    FtpPassword,
                    SshHost,
                    SshUser,
                    SshPassword
                )
            );

    Target Publish =>
        _ => _.DependsOn(ProdPublishBrowser)
            .Executes(() =>
                {
                    var botClient = new TelegramBotClient(TelegramToken);
                    botClient.SendTextMessageAsync(
                            chatId: "@spravy_release",
                            text: $"Published v{Version}"
                        )
                        .GetAwaiter()
                        .GetResult();
                }
            );

    void DeployDesktop(string runtime, string ftpHost, string ftpUser, string fptPassword)
    {
        using var ftpClient = CreateFtpClient(ftpHost, ftpUser, fptPassword);
        ftpClient.Connect();
        var desktop = Solution.AllProjects.Single(x => x.Name == "Spravy.Ui.Desktop").ThrowIfNull();
        var desktopFolder = desktop.PublishProject(PathHelper.PublishFolder.Combine(runtime), Configuration,
            settings => settings.SetRuntime(runtime)
        );
        ftpClient.DeleteIfExistsFolder($"/home/{ftpUser}/Apps/Spravy.Ui.Desktop".ToFolder().Combine(runtime));
        ftpClient.CreateIfNotExistsDirectory($"/home/{ftpUser}/Apps/Spravy.Ui.Desktop".ToFolder().Combine(runtime));
        ftpClient.UploadDirectory(desktopFolder.FullName, $"/home/{ftpUser}/Apps/Spravy.Ui.Desktop/{runtime}");
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

    ConnectionInfo CreateSshConnection(string sshHost, string sshUser, string sshPassword)
    {
        var values = sshHost.Split(":");
        var password = new PasswordAuthenticationMethod(sshUser, sshPassword);

        if (values.Length == 2)
        {
            return new ConnectionInfo(values[0], int.Parse(values[1]), sshUser, password);
        }

        return new ConnectionInfo(values[0], sshUser, password);
    }

    FtpClient CreateFtpClient(string ftpHost, string ftpUser, string fptPassword)
    {
        var values = ftpHost.Split(":");

        if (values.Length == 2)
        {
            return new FtpClient(values[0], ftpUser, fptPassword, int.Parse(values[1]));
        }

        return new FtpClient(ftpHost, ftpUser, fptPassword);
    }
}
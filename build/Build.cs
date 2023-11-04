using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
                        Hosts[serviceProject.GetOptionsName()] = $"http://{ServerHost}:{port}";
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
                        project.SetGetAppSettingsFile(Token, ServiceOptions, Hosts);
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
                    foreach (var project in Solution.AllProjects.Where(x => !x.Name.Contains("Android")))
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
                    using var ftpClient = CreateFtpClient();
                    ftpClient.Connect();
                    var desktop = Solution.AllProjects.Single(x => x.Name == "Spravy.Ui.Desktop").ThrowIfNull();
                    var desktopFolder = desktop.PublishProject(PathHelper.PublishFolder, Configuration);
                    ftpClient.DeleteIfExistsFolder($"/home/{FtpUser}/Apps/Spravy.Ui.Desktop".ToFolder());
                    ftpClient.CreateIfNotExistsDirectory($"/home/{FtpUser}/Apps".ToFolder());
                    ftpClient.UploadDirectory(desktopFolder.FullName, $"/home/{FtpUser}/Apps/Spravy.Ui.Desktop");
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
                    var name = browserProject.Name;
                    var folder = browserProject.PublishProject(PathHelper.PublishFolder, Configuration);
                    var appBundlePath = "bin/Release/net7.0/browser-wasm/AppBundle";
                    var appBundleFolder = Path.Combine(browserProject.Directory, appBundlePath).ToFolder();
                    CopyDirectory(appBundleFolder.FullName, Path.Combine(folder.FullName, "AppBundle"), true);
                    ftpClient.DeleteIfExistsFolder($"/home/{FtpUser}/{name}".ToFolder());
                    ftpClient.UploadDirectory(folder.FullName, $"/home/{FtpUser}/{name}");
                    sshClient.SafeRun($"echo {SshPassword} | sudo -S chown -R nginx /home/{FtpUser}/{name}");
                    sshClient.SafeRun($"echo {SshPassword} | sudo -S chmod -R 777 /home/{FtpUser}/{name}");
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

                    if (keyStoreFile.Exists)
                    {
                        keyStoreFile.Delete();
                    }

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

                    var android = Solution.AllProjects.Single(x => x.Name == "Spravy.Ui.Android");

                    var androidFolder = android.PublishProject(PathHelper.PublishFolder, Configuration, setting =>
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
                    ftpClient.UploadDirectory(androidFolder.FullName, $"/home/{FtpUser}/Apps/Spravy.Ui.Android");
                }
            );

    Target Publish => _ => _.DependsOn(PublishDesktop, PublishAndroid, PublishBrowser);

    void SetServiceSettings(
        Stream stream,
        JsonDocument jsonDocument,
        ushort port,
        Dictionary<string, string> hosts,
        string token
    )
    {
        var jsonWriterOptions = new JsonWriterOptions
        {
            Indented = true,
        };

        using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);
        writer.WriteStartObject();

        foreach (var obj in jsonDocument.RootElement.EnumerateObject())
        {
            if (hosts.TryGetValue(obj.Name, out var host))
            {
                writer.WritePropertyName(obj.Name);
                writer.WriteStartObject();
                writer.WritePropertyName("Host");
                writer.WriteStringValue(host);
                writer.WritePropertyName("ChannelType");
                writer.WriteStringValue("Default");
                writer.WritePropertyName("ChannelCredentialType");
                writer.WriteStringValue("Insecure");
                writer.WritePropertyName("Token");
                writer.WriteStringValue(token);
                writer.WriteEndObject();

                continue;
            }

            if (obj.Name == "Urls")
            {
                writer.WritePropertyName("Urls");
                writer.WriteStringValue($"http://0.0.0.0:{port}");

                continue;
            }

            if (obj.Name == "Serilog")
            {
                writer.WritePropertyName("Serilog");
                writer.WriteStartObject();

                foreach (var ser in obj.Value.EnumerateObject())
                {
                    if (ser.Name == "MinimumLevel")
                    {
                        writer.WritePropertyName("MinimumLevel");
                        writer.WriteStartObject();
                        writer.WritePropertyName("Default");
                        writer.WriteStringValue("Information");
                        writer.WritePropertyName("Override");
                        writer.WriteStartObject();
                        writer.WritePropertyName("Microsoft");
                        writer.WriteStringValue("Warning");
                        writer.WriteEndObject();
                        writer.WriteEndObject();

                        continue;
                    }

                    ser.WriteTo(writer);
                }

                writer.WriteEndObject();

                continue;
            }

            obj.WriteTo(writer);
        }

        writer.WriteEndObject();
    }

    void SetServiceSettings(
        FileInfo appSettingFile,
        ushort port,
        Dictionary<string, string> hosts,
        string token
    )
    {
        var jsonDocument = JsonDocument.Parse(File.ReadAllText(appSettingFile.FullName));
        using var stream = new MemoryStream();
        SetServiceSettings(stream, jsonDocument, port, hosts, token);
        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(appSettingFile.FullName, jsonData);
    }

    string CreateDaemonConfig(string serviceName)
    {
        return $"""
                [Unit]
                Description={serviceName}
                After=network.target

                [Service]
                WorkingDirectory=/home/{FtpUser}/{serviceName}
                ExecStart=/usr/bin/dotnet /home/{FtpUser}/{serviceName}/{serviceName}.dll
                Restart=always
                # Restart service after 10 seconds if the dotnet service crashes:
                RestartSec=10
                KillSignal=SIGINT
                SyslogIdentifier={serviceName.ToLower().Replace(",", "-")}
                User={FtpUser}
                Environment=ASPNETCORE_ENVIRONMENT=Production
                Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

                [Install]
                WantedBy=multi-user.target
                """;
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
            client.DeleteDirectory(path,
                FtpListOption.Recursive | FtpListOption.ForceList | FtpListOption.Auto | FtpListOption.AllFiles
            );
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
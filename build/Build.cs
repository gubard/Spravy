using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using _build.Models;
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
    [Parameter] readonly string ServerHost;
    static readonly DirectoryInfo TempFolder = new(Path.Combine("/", "tmp", "Spravy"));
    static readonly DirectoryInfo PublishFolder = new(Path.Combine(TempFolder.FullName, "Publish"));
    static readonly DirectoryInfo ServicesFolder = new(Path.Combine(TempFolder.FullName, "services"));

    [Solution] readonly Solution Solution;

    Target Clean =>
        _ => _
            .Before(Restore)
            .Executes(() => DotNetClean(setting => setting.SetProject(Solution).SetConfiguration(Configuration)));

    Target Restore =>
        _ => _
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

    Target Publish =>
        _ => _
            .DependsOn(Compile)
            .Executes(() =>
                {
                    var serviceProjects =
                        Solution.AllProjects.Where(x => x.Name.EndsWith(".Service") && x.Name != "Spravy.Service")
                            .ToArray();

                    var browserProject = Solution.AllProjects.Single(x => x.Name == "Spravy.Ui.Browser");
                    ushort port = 5000;
                    ushort browserPort = 6000;
                    var serviceOptions = new Dictionary<Project, ServiceOptions>();
                    var hosts = new Dictionary<string, string>();
                    var browserHosts = new Dictionary<string, string>();

                    foreach (var serviceProject in serviceProjects)
                    {
                        serviceOptions[serviceProject] = new ServiceOptions(
                            port,
                            browserPort,
                            serviceProject.Name,
                            $"{serviceProject.Name}.Browser"
                        );

                        hosts[$"Grpc{serviceProject.Name.Substring(6).Replace(".", "")}"] =
                            $"http://{ServerHost}:{port}";

                        browserHosts[$"Grpc{serviceProject.Name.Substring(6).Replace(".", "")}"] =
                            $"http://{ServerHost}:{browserPort}";

                        port++;
                        browserPort++;
                    }

                    using var sshClient = new SshClient(CreateSshConnection());
                    sshClient.Connect();
                    using var ftpClient = CreateFtpClient();
                    ftpClient.Connect();
                    var token = CreteToken();

                    foreach (var serviceOption in serviceOptions)
                    {
                        PublishService(
                            serviceOption.Key,
                            serviceOption.Value.ServiceName,
                            serviceOption.Value.Port,
                            "Http2",
                            sshClient,
                            ftpClient,
                            hosts,
                            token
                        );
                        PublishService(
                            serviceOption.Key,
                            serviceOption.Value.BrowserServiceName,
                            serviceOption.Value.BrowserPort,
                            "HttpAndHttp2",
                            sshClient,
                            ftpClient,
                            hosts,
                            token
                        );
                    }

                    PublishService(browserProject,
                        browserProject.Name,
                        browserPort,
                        "HttpAndHttp2",
                        sshClient,
                        ftpClient,
                        browserHosts,
                        token
                    );

                    using var daemonReloadCommand =
                        sshClient.RunCommand($"echo {SshPassword} | sudo systemctl daemon-reload");

                    foreach (var serviceProject in serviceProjects)
                    {
                        using var sshCommand =
                            sshClient.RunCommand(
                                $"echo {SshPassword} | sudo systemctl restart {serviceProject.Name.ToLower()}"
                            );
                    }

                    var desktop = Solution.AllProjects.Single(x => x.Name == "Spravy.Ui.Desktop");
                    var desktopFolder = PublishProject(desktop, desktop.Name);
                    var desktopAppFolder = new DirectoryInfo($"/home/{FtpUser}/Apps/Spravy.Ui.Desktop");
                    var desktopAppSettings = new FileInfo(Path.Combine(desktopAppFolder.FullName, "appsettings.json"));

                    if (desktopAppFolder.Exists)
                    {
                        desktopAppFolder.Delete(true);
                    }

                    CopyDirectory(desktopFolder.FullName, desktopAppFolder.FullName, true);
                    SetServiceSettings(desktopAppSettings, 0, null, hosts, "");
                }
            );

    void PublishService(
        Project project,
        string name,
        ushort port,
        string protocol,
        SshClient sshClient,
        FtpClient ftpClient,
        Dictionary<string, string> hosts,
        string token
    )
    {
        var folder = PublishProject(project, name);
        var appSettingsFile = new FileInfo(Path.Combine(folder.FullName, "appsettings.json"));
        SetServiceSettings(appSettingsFile, port, protocol, hosts, token);
        DeleteIfExistsDirectory(ftpClient, $"/home/{FtpUser}/{name}");
        ftpClient.UploadDirectory(folder.FullName, $"/home/{FtpUser}/{name}");

        using var rmCommand =
            sshClient.RunCommand(
                $"echo {SshPassword} | rm /etc/systemd/system/{name.ToLower()}"
            );

        if (!ServicesFolder.Exists)
        {
            ServicesFolder.Create();
        }

        File.WriteAllText(Path.Combine(ServicesFolder.FullName, name.ToLower()),
            CreateDaemonConfig(name)
        );

        if (!ftpClient.DirectoryExists("/tmp/Spravy/services"))
        {
            ftpClient.CreateDirectory("/tmp/Spravy/services");
        }

        ftpClient.UploadFile(Path.Combine(ServicesFolder.FullName, name.ToLower()),
            $"/tmp/Spravy/services/{name.ToLower()}"
        );

        using var cpCommand =
            sshClient.RunCommand(
                $"echo {SshPassword} | sudo cp /tmp/Spravy/services/{name.ToLower()} /etc/systemd/system/{name.ToLower()}"
            );
    }

    void SetServiceSettings(
        Stream stream,
        JsonDocument jsonDocument,
        ushort port,
        string protocol,
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

            if (obj.Name == "Kestrel")
            {
                writer.WritePropertyName("Kestrel");
                writer.WriteStartObject();
                writer.WritePropertyName("EndpointDefaults");
                writer.WriteStartObject();
                writer.WritePropertyName("Protocols");
                writer.WriteStringValue(protocol);
                writer.WriteEndObject();
                writer.WriteEndObject();

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
        string protocol,
        Dictionary<string, string> hosts,
        string token
    )
    {
        var jsonDocument = JsonDocument.Parse(File.ReadAllText(appSettingFile.FullName));
        using var stream = new MemoryStream();
        SetServiceSettings(stream, jsonDocument, port, protocol, hosts, token);
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

    static void DeleteIfExistsDirectory(FtpClient client, string path)
    {
        if (!client.DirectoryExists(path))
        {
            return;
        }

        client.DeleteDirectory(path, FtpListOption.Recursive);
    }

    static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
        }

        // Cache directories before we start copying
        var dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (var file in dir.GetFiles())
        {
            var targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
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

    DirectoryInfo PublishProject(Project project, string name, Action<DotNetPublishSettings> configurator = null)
    {
        var publishFolder = new DirectoryInfo(Path.Combine(PublishFolder.FullName, name));

        if (publishFolder.Exists)
        {
            publishFolder.Delete(true);
        }

        if (!publishFolder.Exists)
        {
            publishFolder.Create();
        }

        for (var i = 0; i < 3; i++)
        {
            try
            {
                DotNetPublish(setting =>
                    {
                        configurator?.Invoke(setting);

                        return setting.SetConfiguration(Configuration)
                            .SetProject(project)
                            .SetOutput(publishFolder.FullName)
                            .EnableNoBuild()
                            .EnableNoRestore();
                    }
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

        return publishFolder;
    }

    string CreteToken()
    {
        var key = new SymmetricSecurityKey("0bf7731f-2441-4cff-8e2e-7b343d5d35d0b9b47d13-5b69-4249-aed9-24421e8a94d9"u8
            .ToArray()
        );

        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        var token = new JwtSecurityToken(
            "https://spravy.issuer.authentication.com",
            "https://spravy.audience.authentication.com",
            new List<Claim>
            {
                new(ClaimTypes.Role, "Service"),
            },
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: signingCredentials
        );

        var jwt = jwtSecurityTokenHandler.WriteToken(token);

        return jwt;
    }

    ConnectionInfo CreateSshConnection()
    {
        var values = FtpHost.Split(":");

        if (values.Length == 2)
        {
            return new ConnectionInfo(values[0], int.Parse(values[1]), FtpUser,
                new PasswordAuthenticationMethod(FtpUser, SshPassword)
            );
        }

        return new ConnectionInfo(values[0], FtpUser, new PasswordAuthenticationMethod(FtpUser, SshPassword));
    }

    FtpClient CreateFtpClient()
    {
        var values = FtpHost.Split(":");

        if (values.Length == 2)
        {
            return new FtpClient(values[0], FtpUser, FtpPassword, int.Parse(values[1]));
        }

        return new FtpClient(SshHost, FtpUser, FtpPassword);
    }
}
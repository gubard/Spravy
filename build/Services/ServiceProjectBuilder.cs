using System.IO;
using System.Text;
using _build.Extensions;
using _build.Helpers;
using _build.Models;
using Nuke.Common.Tools.DotNet;
using Serilog;

namespace _build.Services;

public class ServiceProjectBuilder : ProjectBuilder
{
    public ServiceProjectBuilder(
        VersionService versionService,
        ServiceProjectBuilderOptions serviceOptions
    ) : base(serviceOptions, versionService)
    {
        this.ServiceOptions = serviceOptions;
    }

    public ServiceProjectBuilderOptions ServiceOptions { get; }

    public override void Setup()
    {
        var jsonDocument = options.AppSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        Log.Logger.Information("Set app settings {File}", options.AppSettingsFile);

        stream.SetAppSettingsStream(
            jsonDocument,
            options.Domain,
            options.Hosts,
            ServiceOptions.Token,
            ServiceOptions.Port,
            ServiceOptions.EmailPassword
        );

        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(options.AppSettingsFile.FullName, jsonData);
    }

    public void Publish()
    {
        if (options.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(options.Configuration)
                .SetProject(options.CsprojFile.FullName)
                .SetOutput(ServiceOptions.PublishFolder.FullName)
                .EnableNoBuild()
                .EnableNoRestore()
            );
        }
        else
        {
            foreach (var runtime in options.Runtimes.Span)
            {
                DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(options.Configuration)
                    .SetProject(options.CsprojFile.FullName)
                    .SetOutput(ServiceOptions.PublishFolder.Combine(runtime.Name).FullName)
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .SetRuntime(runtime.Name)
                );
            }
        }

        using var sshClient = ServiceOptions.CreateSshClient();
        sshClient.Connect();
        using var ftpClient = ServiceOptions.CreateFtpClient();
        ftpClient.Connect();
        ftpClient.DeleteIfExistsFolder(ServiceOptions.GetAppFolder());

        if (options.Runtimes.IsEmpty)
        {
            ftpClient.UploadDirectory(
                ServiceOptions.PublishFolder.FullName,
                ServiceOptions.GetAppFolder().FullName
            );
        }
        else
        {
            ftpClient.UploadDirectory(
                ServiceOptions.PublishFolder.Combine(ServiceOptions.Runtime.Name).FullName,
                ServiceOptions.GetAppFolder().FullName
            );
        }

        sshClient.SafeRun(
            $"echo {ServiceOptions.SshPassword} | sudo -S rm /etc/systemd/system/{options.GetServiceName()}"
        );

        PathHelper.ServicesFolder.CreateIfNotExits();

        var serviceFile =
            PathHelper.ServicesFolder.ToFile(options.GetServiceName());

        serviceFile.WriteAllText(GetDaemonConfig());
        ftpClient.CreateIfNotExistsFolder(PathHelper.ServicesFolder);
        ftpClient.UploadFile(serviceFile.FullName, serviceFile.FullName);

        sshClient.SafeRun(
            $"echo {ServiceOptions.SshPassword} | sudo -S cp {serviceFile} /etc/systemd/system/{options.GetServiceName()}"
        );
    }

    string GetDaemonConfig()
    {
        return $"""
                [Unit]
                Description={options.GetProjectName()}
                After=network.target

                [Service]
                WorkingDirectory={ServiceOptions.GetAppFolder()}
                ExecStart=/usr/bin/dotnet {ServiceOptions.GetAppDll()}
                Restart=always
                # Restart service after 10 seconds if the dotnet service crashes:
                RestartSec=10
                KillSignal=SIGINT
                SyslogIdentifier={options.GetServiceName().Replace(",", "-")}
                User={ServiceOptions.FtpUser}
                Environment=ASPNETCORE_ENVIRONMENT=Production
                Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

                [Install]
                WantedBy=multi-user.target
                """;
    }
}
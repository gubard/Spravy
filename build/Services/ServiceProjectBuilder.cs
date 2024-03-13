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
    readonly ServiceProjectBuilderOptions serviceOptions;

    public ServiceProjectBuilder(
        VersionService versionService,
        ServiceProjectBuilderOptions serviceOptions
    ) : base(serviceOptions, versionService)
    {
        this.serviceOptions = serviceOptions;
    }

    public override void Setup()
    {
        var jsonDocument = options.AppSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        Log.Logger.Information("Set app settings {File}", options.AppSettingsFile);

        stream.SetAppSettingsStream(
            jsonDocument,
            options.Domain,
            options.Hosts,
            serviceOptions.Token,
            serviceOptions.Port,
            serviceOptions.EmailPassword
        );

        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(options.AppSettingsFile.FullName, jsonData);
    }

    public void Publish()
    {
        if (options.Runtimes.IsEmpty)
        {
            serviceOptions.PublishFolder.DeleteIfExits();

            DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(options.Configuration)
                .SetProject(options.CsprojFile.FullName)
                .SetOutput(serviceOptions.PublishFolder.FullName)
                .EnableNoBuild()
                .EnableNoRestore()
            );
        }
        else
        {
            foreach (var runtime in options.Runtimes.Span)
            {
                var output = serviceOptions.PublishFolder.Combine(runtime.Name);
                output.DeleteIfExits();

                DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(options.Configuration)
                    .SetProject(options.CsprojFile.FullName)
                    .SetOutput(serviceOptions.PublishFolder.Combine(runtime.Name).FullName)
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .SetRuntime(runtime.Name)
                );
            }
        }

        using var sshClient = serviceOptions.CreateSshClient();
        sshClient.Connect();
        using var ftpClient = serviceOptions.CreateFtpClient();
        ftpClient.Connect();

        ftpClient.DeleteIfExistsFolder(serviceOptions.GetAppFolder());

        if (options.Runtimes.IsEmpty)
        {
            ftpClient.UploadDirectory(
                serviceOptions.PublishFolder.FullName,
                serviceOptions.GetAppFolder().FullName
            );
        }
        else
        {
            ftpClient.UploadDirectory(
                serviceOptions.PublishFolder.Combine(serviceOptions.Runtime.Name).FullName,
                serviceOptions.GetAppFolder().FullName
            );
        }

        sshClient.SafeRun(
            $"echo {serviceOptions.SshPassword} | sudo -S rm /etc/systemd/system/{options.GetServiceName()}"
        );

        PathHelper.ServicesFolder.CreateIfNotExits();

        var serviceFile =
            PathHelper.ServicesFolder.ToFile(options.GetServiceName());

        serviceFile.WriteAllText(GetDaemonConfig());
        ftpClient.CreateIfNotExistsDirectory(PathHelper.ServicesFolder);
        ftpClient.UploadFile(serviceFile.FullName, serviceFile.FullName);

        sshClient.SafeRun(
            $"echo {serviceOptions.SshPassword} | sudo -S cp {serviceFile} /etc/systemd/system/{options.GetServiceName()}"
        );

        sshClient.SafeRun(
            $"echo {serviceOptions.SshPassword} | sudo -S systemctl enable {options.GetServiceName()}"
        );
        sshClient.SafeRun(
            $"echo {serviceOptions.SshPassword} | sudo -S systemctl restart {options.GetServiceName()}"
        );
    }

    string GetDaemonConfig()
    {
        return $"""
                [Unit]
                Description={options.GetProjectName()}
                After=network.target

                [Service]
                WorkingDirectory={serviceOptions.GetAppFolder()}
                ExecStart=/usr/bin/dotnet {serviceOptions.GetAppDll()}
                Restart=always
                # Restart service after 10 seconds if the dotnet service crashes:
                RestartSec=10
                KillSignal=SIGINT
                SyslogIdentifier={options.GetServiceName().Replace(",", "-")}
                User={serviceOptions.FtpUser}
                Environment=ASPNETCORE_ENVIRONMENT=Production
                Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

                [Install]
                WantedBy=multi-user.target
                """;
    }
}
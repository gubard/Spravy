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
    readonly ServiceProjectBuilderOptions serviceProjectBuilderOptions;

    public ServiceProjectBuilder(
        VersionService versionService,
        ServiceProjectBuilderOptions serviceProjectBuilderOptions
    ) : base(serviceProjectBuilderOptions, versionService)
    {
        this.serviceProjectBuilderOptions = serviceProjectBuilderOptions;
    }

    public override void Setup()
    {
        var jsonDocument = projectBuilderOptions.AppSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        Log.Logger.Information("Set app settings {File}", projectBuilderOptions.AppSettingsFile);

        stream.SetAppSettingsStream(
            jsonDocument,
            projectBuilderOptions.Domain,
            projectBuilderOptions.Hosts,
            serviceProjectBuilderOptions.Token,
            serviceProjectBuilderOptions.Port,
            serviceProjectBuilderOptions.EmailPassword
        );

        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(projectBuilderOptions.AppSettingsFile.FullName, jsonData);
    }

    public void Publish()
    {
        serviceProjectBuilderOptions.PublishFolder.DeleteIfExits();
        using var sshClient = serviceProjectBuilderOptions.CreateSshClient();
        sshClient.Connect();
        using var ftpClient = serviceProjectBuilderOptions.CreateFtpClient();
        ftpClient.Connect();

        if (projectBuilderOptions.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(projectBuilderOptions.Configuration)
                .SetProject(projectBuilderOptions.CsprojFile.FullName)
                .SetOutput(serviceProjectBuilderOptions.PublishFolder.FullName)
                .EnableNoBuild()
                .EnableNoRestore()
            );
        }
        else
        {
            foreach (var runtime in projectBuilderOptions.Runtimes.Span)
            {
                DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(projectBuilderOptions.Configuration)
                    .SetProject(projectBuilderOptions.CsprojFile.FullName)
                    .SetOutput(serviceProjectBuilderOptions.PublishFolder.FullName)
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .SetRuntime(runtime.Name)
                );
            }
        }

        ftpClient.DeleteIfExistsFolder(
            $"/home/{serviceProjectBuilderOptions.FtpUser}/{projectBuilderOptions.GetProjectName()}"
                .ToFolder()
        );

        ftpClient.UploadDirectory(serviceProjectBuilderOptions.PublishFolder.FullName,
            $"/home/{serviceProjectBuilderOptions.FtpUser}/{projectBuilderOptions.GetProjectName()}"
        );

        sshClient.SafeRun(
            $"echo {serviceProjectBuilderOptions.SshPassword} | sudo -S rm /etc/systemd/system/{projectBuilderOptions.GetProjectName().ToLower()}"
        );

        PathHelper.ServicesFolder.CreateIfNotExits();

        var serviceFile =
            PathHelper.ServicesFolder.ToFile(projectBuilderOptions.GetProjectName().ToLower());

        serviceFile.WriteAllText(GetDaemonConfig());
        ftpClient.CreateIfNotExistsDirectory(PathHelper.ServicesFolder);
        ftpClient.UploadFile(serviceFile.FullName, serviceFile.FullName);

        sshClient.SafeRun(
            $"echo {serviceProjectBuilderOptions.SshPassword} | sudo -S cp {serviceFile} /etc/systemd/system/{projectBuilderOptions.GetProjectName().ToLower()}"
        );

        sshClient.SafeRun(
            $"echo {serviceProjectBuilderOptions.SshPassword} | sudo -S systemctl enable {projectBuilderOptions.GetProjectName().ToLower()}"
        );
        sshClient.SafeRun(
            $"echo {serviceProjectBuilderOptions.SshPassword} | sudo -S systemctl restart {projectBuilderOptions.GetProjectName().ToLower()}"
        );
    }

    string GetDaemonConfig()
    {
        return $"""
                [Unit]
                Description={projectBuilderOptions.GetProjectName()}
                After=network.target

                [Service]
                WorkingDirectory=/home/{serviceProjectBuilderOptions.FtpUser}/{projectBuilderOptions.GetProjectName()}
                ExecStart=/usr/bin/dotnet /home/{serviceProjectBuilderOptions.FtpUser}/{projectBuilderOptions.GetProjectName()}/{projectBuilderOptions.GetProjectName()}.dll
                Restart=always
                # Restart service after 10 seconds if the dotnet service crashes:
                RestartSec=10
                KillSignal=SIGINT
                SyslogIdentifier={projectBuilderOptions.GetProjectName().ToLower().Replace(",", "-")}
                User={serviceProjectBuilderOptions.FtpUser}
                Environment=ASPNETCORE_ENVIRONMENT=Production
                Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

                [Install]
                WantedBy=multi-user.target
                """;
    }
}
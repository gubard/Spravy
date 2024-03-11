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
        ProjectBuilderOptions projectBuilderOptions,
        VersionService versionService,
        ServiceProjectBuilderOptions serviceProjectBuilderOptions
    ) : base(projectBuilderOptions, versionService)
    {
        this.serviceProjectBuilderOptions = serviceProjectBuilderOptions;
    }

    public override void Setup(string host)
    {
        var jsonDocument = projectBuilderOptions.AppSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        Log.Logger.Information("Set app settings {File}", projectBuilderOptions.AppSettingsFile);

        stream.SetAppSettingsStream(
            jsonDocument,
            host,
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
            $"/home/{serviceProjectBuilderOptions.FtpUser}/{projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension()}"
                .ToFolder()
        );

        ftpClient.UploadDirectory(serviceProjectBuilderOptions.PublishFolder.FullName,
            $"/home/{serviceProjectBuilderOptions.FtpUser}/{projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension()}"
        );

        sshClient.SafeRun(
            $"echo {serviceProjectBuilderOptions.SshPassword} | sudo -S rm /etc/systemd/system/{projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension().ToLower()}"
        );

        PathHelper.ServicesFolder.CreateIfNotExits();

        var serviceFile =
            PathHelper.ServicesFolder.ToFile(projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension().ToLower());

        serviceFile.WriteAllText(GetDaemonConfig());
        ftpClient.CreateIfNotExistsDirectory(PathHelper.ServicesFolder);
        ftpClient.UploadFile(serviceFile.FullName, serviceFile.FullName);

        sshClient.SafeRun(
            $"echo {serviceProjectBuilderOptions.SshPassword} | sudo -S cp {serviceFile} /etc/systemd/system/{projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension().ToLower()}"
        );

        sshClient.SafeRun(
            $"echo {serviceProjectBuilderOptions.SshPassword} | sudo -S systemctl enable {projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension().ToLower()}"
        );
        sshClient.SafeRun(
            $"echo {serviceProjectBuilderOptions.SshPassword} | sudo -S systemctl restart {projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension().ToLower()}"
        );
    }

    string GetDaemonConfig()
    {
        return $"""
                [Unit]
                Description={projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension()}
                After=network.target

                [Service]
                WorkingDirectory=/home/{serviceProjectBuilderOptions.FtpUser}/{projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension()}
                ExecStart=/usr/bin/dotnet /home/{serviceProjectBuilderOptions.FtpUser}/{projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension()}/{projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension()}.dll
                Restart=always
                # Restart service after 10 seconds if the dotnet service crashes:
                RestartSec=10
                KillSignal=SIGINT
                SyslogIdentifier={projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension().ToLower().Replace(",", "-")}
                User={serviceProjectBuilderOptions.FtpUser}
                Environment=ASPNETCORE_ENVIRONMENT=Production
                Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

                [Install]
                WantedBy=multi-user.target
                """;
    }
}
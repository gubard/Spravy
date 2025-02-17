using System.Diagnostics;
using System.IO;
using System.Text;
using _build.Extensions;
using _build.Helpers;
using _build.Models;
using Nuke.Common.Tools.DotNet;
using Serilog;

namespace _build.Services;

public class ServiceProjectBuilder : ProjectBuilder<ServiceProjectBuilderOptions>
{
    public ServiceProjectBuilder(SpravyVersion version, ServiceProjectBuilderOptions serviceOptions) : base(
        serviceOptions,
        version
    )
    {
    }

    public override void Setup()
    {
        Log.Logger.Information("Set app settings {File}", Options.AppSettingsFile);
        var jsonDocument = Options.AppSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();

        stream.SetAppSettingsStream(
            jsonDocument,
            Options.Domain,
            Options.Hosts,
            Options.Token,
            Options.Port,
            Options.EmailPassword
        );

        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(Options.AppSettingsFile.FullName, jsonData);
    }

    public void Publish()
    {
        if (Options.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetPublish(
                setting => setting.SetConfiguration(Options.Configuration)
                   .SetProject(Options.CsprojFile.FullName)
                   .SetOutput(Options.PublishFolder.FullName)
                   .EnableNoBuild()
                   .EnableNoRestore()
            );
        }
        else
        {
            foreach (var runtime in Options.Runtimes.Span)
            {
                DotNetTasks.DotNetPublish(
                    setting => setting.SetConfiguration(Options.Configuration)
                       .SetProject(Options.CsprojFile.FullName)
                       .SetOutput(Options.PublishFolder.Combine(runtime.Name).FullName)
                       .EnableNoBuild()
                       .EnableNoRestore()
                       .SetRuntime(runtime.Name)
                );
            }
        }

        using var sshClient = Options.CreateSshClient();
        sshClient.Connect();
        using var ftpClient = Options.CreateFtpClient();
        ftpClient.Connect();
        ftpClient.DeleteIfExistsFolder(Options.GetAppFolder());

        if (Options.Runtimes.IsEmpty)
        {
            ftpClient.UploadDirectory(Options.PublishFolder.FullName, Options.GetAppFolder().FullName);
        }
        else
        {
            ftpClient.UploadDirectory(
                Options.PublishFolder.Combine(Options.Runtime.Name).FullName,
                Options.GetAppFolder().FullName
            );
        }

        sshClient.RunSudo(Options, $"rm /etc/systemd/system/{Options.GetServiceName()}");
        PathHelper.ServicesFolder.CreateIfNotExits();
        var serviceFile = PathHelper.ServicesFolder.ToFile(Options.GetServiceName());
        serviceFile.WriteAllText(GetDaemonConfig());
        ftpClient.CreateIfNotExistsFolder(PathHelper.ServicesFolder);
        ftpClient.UploadFile(serviceFile.FullName, serviceFile.FullName);
        sshClient.RunSudo(Options, $"cp {serviceFile} /etc/systemd/system/{Options.GetServiceName()}");
    }

    public void BuildDocker()
    {
        var process = Process.Start(
                new ProcessStartInfo(
                    "docker",
                    $"build {Options.CsprojFile.Directory.Combine("..")} -f {Options.CsprojFile.Directory.ToFile("Dockerfile")} -t {Options.GetProjectName().ToLower()}:{version}"
                )
            )
           .ThrowIfNull();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new($"ExitCode {process.ExitCode}");
        }
    }

    public void PushDockerImage()
    {
        var processTag = Process.Start(
                new ProcessStartInfo(
                    "docker",
                    $"tag {Options.GetProjectName().ToLower()}:{version} 192.168.50.45:5000/myfirstimage/{Options.GetProjectName().ToLower()}:{version}"
                )
            )
           .ThrowIfNull();

        processTag.WaitForExit();

        if (processTag.ExitCode != 0)
        {
            throw new($"Tag ExitCode {processTag.ExitCode}");
        }

        var processPush = Process.Start(
                new ProcessStartInfo(
                    "docker",
                    $"image push 192.168.50.45:5000/myfirstimage/{Options.GetProjectName().ToLower()}:{version}"
                )
            )
           .ThrowIfNull();

        processPush.WaitForExit();

        if (processPush.ExitCode != 0)
        {
            throw new($"Push ExitCode {processPush.ExitCode}");
        }
    }

    string GetDaemonConfig() =>
        $"""
        [Unit]
        Description={Options.GetProjectName()}
        After=network.target

        [Service]
        WorkingDirectory={Options.GetAppFolder()}
        ExecStart=/usr/bin/dotnet {Options.GetAppDll()}
        Restart=always
        # Restart service after 10 seconds if the dotnet service crashes:
        RestartSec=10
        KillSignal=SIGINT
        SyslogIdentifier={Options.GetServiceName().Replace(",", "-")}
        User={Options.FtpUser}
        Environment=ASPNETCORE_ENVIRONMENT=Production
        Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

        [Install]
        WantedBy=multi-user.target
        """;
}
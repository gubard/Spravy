using System;
using System.Collections.Generic;
using System.IO;
using _build.Helpers;
using _build.Models;
using FluentFTP;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Renci.SshNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace _build.Extensions;

public static class ProjectExtension
{
    public static DirectoryInfo PublishProject(
        this Project project,
        DirectoryInfo publishFolder,
        string configuration,
        Func<DotNetPublishSettings, DotNetPublishSettings> configurator = null
    )
    {
        var resultFolder = new DirectoryInfo(Path.Combine(publishFolder.FullName, project.Name));

        if (resultFolder.Exists)
        {
            resultFolder.Delete(true);
        }

        if (!resultFolder.Exists)
        {
            resultFolder.Create();
        }

        for (var i = 0; i < 3; i++)
        {
            try
            {
                DotNetPublish(setting =>
                    {
                        var result = setting.SetConfiguration(configuration)
                            .SetProject(project)
                            .SetOutput(resultFolder.FullName)
                            .EnableNoBuild()
                            .EnableNoRestore();

                        if (configurator is not null)
                        {
                            result = configurator.Invoke(result);
                        }

                        return result;
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

        return resultFolder;
    }

    public static string GetDaemonConfig(this Project project, string ftpUser)
    {
        return $"""
                [Unit]
                Description={project.Name}
                After=network.target

                [Service]
                WorkingDirectory=/home/{ftpUser}/{project.Name}
                ExecStart=/usr/bin/dotnet /home/{ftpUser}/{project.Name}/{project.Name}.dll
                Restart=always
                # Restart service after 10 seconds if the dotnet service crashes:
                RestartSec=10
                KillSignal=SIGINT
                SyslogIdentifier={project.Name.ToLower().Replace(",", "-")}
                User={ftpUser}
                Environment=ASPNETCORE_ENVIRONMENT=Production
                Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

                [Install]
                WantedBy=multi-user.target
                """;
    }

    public static void DeployService(
        this Project project,
        SshClient sshClient,
        FtpClient ftpClient,
        DirectoryInfo publishFolder,
        string configuration,
        string ftpUser,
        string sshPassword,
        Func<DotNetPublishSettings, DotNetPublishSettings> configurator = null
    )
    {
        var folder = project.PublishProject(publishFolder, configuration, configurator);
        ftpClient.DeleteIfExistsFolder($"/home/{ftpUser}/{project.Name}".ToFolder());
        ftpClient.UploadDirectory(folder.FullName, $"/home/{ftpUser}/{project.Name}");
        sshClient.SafeRun($"echo {sshPassword} | sudo -S rm /etc/systemd/system/{project.Name.ToLower()}");
        PathHelper.ServicesFolder.CreateIfNotExits();
        var serviceFile = PathHelper.ServicesFolder.ToFile(project.Name.ToLower());
        serviceFile.WriteAllText(project.GetDaemonConfig(ftpUser));
        ftpClient.CreateIfNotExistsDirectory(PathHelper.ServicesFolder);
        ftpClient.UploadFile(serviceFile.FullName, serviceFile.FullName);
        sshClient.SafeRun($"echo {sshPassword} | sudo -S cp {serviceFile} /etc/systemd/system/{project.Name.ToLower()}");
    }

    public static string GetOptionsName(this Project project)
    {
        return $"Grpc{project.Name.Substring(6).Replace(".", "")}";
    }

    public static FileInfo GetAppSettingsFile(this Project project)
    {
        return new FileInfo(Path.Combine(project.Directory, "appsettings.json"));
    }

    public static bool IsUi(this Project project)
    {
        return project.Name.Contains(".Ui.");
    }

    public static void SetGetAppSettingsFile(
        this Project project,
        string tokenValue,
        Dictionary<Project, ServiceOptions> serviceOptions,
        Dictionary<string, string> hosts
    )
    {
        var appSettingsFile = project.GetAppSettingsFile();

        if (!appSettingsFile.Exists)
        {
            return;
        }

        var token = tokenValue;

        if (project.IsUi())
        {
            token = string.Empty;
        }

        if (serviceOptions.TryGetValue(project, out var options))
        {
            appSettingsFile.SetAppSettingsFile($"https://0.0.0.0:{options.Port}", hosts, token);
        }
        else
        {
            appSettingsFile.SetAppSettingsFile(hosts, token);
        }
    }
}
using System;
using System.IO;
using System.Linq;
using FluentFTP;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Renci.SshNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace _build;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Publish);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly string FtpPassword;
    [Parameter] readonly string SshPassword;
    [Parameter] readonly string FtpHost;
    [Parameter] readonly string FtpUser;
    [Parameter] readonly string SshHost;
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

                    using var sshClient = new SshClient(FtpHost, FtpUser, SshPassword);
                    sshClient.Connect();
                    using var ftpClient = new FtpClient(SshHost, FtpUser, FtpPassword);
                    ftpClient.Connect();

                    foreach (var serviceProject in serviceProjects)
                    {
                        var folder = PublishProject(serviceProject.Name);
                        DeleteIfExistsDirectory(ftpClient, $"/home/{FtpUser}/{serviceProject.Name}");
                        ftpClient.UploadDirectory(folder.FullName, $"/home/{FtpUser}/{serviceProject.Name}");

                        using var rmCommand =
                            sshClient.RunCommand(
                                $"echo {SshPassword} | rm /etc/systemd/system/{serviceProject.Name.ToLower()}"
                            );

                        if (!ServicesFolder.Exists)
                        {
                            ServicesFolder.Create();
                        }
                        
                        File.WriteAllText(Path.Combine(ServicesFolder.FullName, serviceProject.Name.ToLower()),
                            CreateDaemonConfig(serviceProject.Name)
                        );
                        
                        if(!ftpClient.DirectoryExists("/tmp/Spravy/services"))
                        {
                            ftpClient.CreateDirectory("/tmp/Spravy/services");
                        }

                        ftpClient.UploadFile(Path.Combine(ServicesFolder.FullName, serviceProject.Name.ToLower()),
                            $"/tmp/Spravy/services/{serviceProject.Name.ToLower()}"
                        );
                        
                        using var cpCommand =
                            sshClient.RunCommand(
                                $"echo {SshPassword} | sudo cp /tmp/Spravy/services/{serviceProject.Name.ToLower()} /etc/systemd/system/{serviceProject.Name.ToLower()}"
                            );
                    }

                    using var daemonReloadCommand =
                        sshClient.RunCommand($"echo {SshPassword} | sudo systemctl daemon-reload");

                    foreach (var serviceProject in serviceProjects)
                    {
                        using var sshCommand =
                            sshClient.RunCommand(
                                $"echo {SshPassword} | sudo systemctl restart {serviceProject.Name.ToLower()}"
                            );
                    }

                    var desktopFolder = PublishProject("Spravy.Ui.Desktop");
                    var desktopAppFolder = new DirectoryInfo($"/home/{FtpUser}/Apps/Spravy.Ui.Desktop");

                    if (desktopAppFolder.Exists)
                    {
                        desktopAppFolder.Delete(true);
                    }

                    CopyDirectory(desktopFolder.FullName, desktopAppFolder.FullName, true);
                }
            );

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

    DirectoryInfo PublishProject(string name, Action<DotNetPublishSettings> configurator = null)
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

        var project = Solution.AllProjects.Single(x => x.Name == name);


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
}
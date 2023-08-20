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

    [Parameter] readonly string AndroidSigningKeyPass;
    [Parameter] readonly string AndroidSigningStorePass;
    [Parameter] readonly string FtpPassword;
    [Parameter] readonly string SshPassword;

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
                    foreach (var project in Solution.Projects.Where(x => !x.Name.Contains("Android")))
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
                    using var sshClient = new SshClient("192.168.50.2", "vafnir", SshPassword);
                    sshClient.Connect();
                    using var ftpClient = new FtpClient("192.168.50.2", "vafnir", FtpPassword);
                    ftpClient.Connect();
                    
                    var authenticationMigratorFolder = PublishProject("Spravy.Authentication.Db.Sqlite.Migrator");
                    ftpClient.DeleteDirectory("/home/vafnir/Spravy.Authentication.Db.Sqlite.Migrator", FtpListOption.Recursive);
                    ftpClient.UploadDirectory(authenticationMigratorFolder.FullName, "/home/vafnir/Spravy.Authentication.Db.Sqlite.Migrator");

                    using var commandAuthenticationMigrator =
                        sshClient.RunCommand(
                            "cd /home/vafnir/Spravy.Authentication.Db.Sqlite.Migrator;dotnet Spravy.Authentication.Db.Sqlite.Migrator.dll"
                        );

                    
                    var toDoMigratorFolder = PublishProject("Spravy.ToDo.Db.Sqlite.Migrator");
                    ftpClient.DeleteDirectory("/home/vafnir/Spravy.ToDo.Db.Sqlite.Migrator", FtpListOption.Recursive);
                    ftpClient.UploadDirectory(toDoMigratorFolder.FullName, "/home/vafnir/Spravy.ToDo.Db.Sqlite.Migrator");

                    using var commandToDoMigrator =
                        sshClient.RunCommand(
                            "cd /home/vafnir/Spravy.ToDo.Db.Sqlite.Migrator;dotnet Spravy.ToDo.Db.Sqlite.Migrator.dll"
                        );

                    var authenticationServiceFolder = PublishProject("Spravy.Authentication.Service");
                    ftpClient.DeleteDirectory("/home/vafnir/Spravy.Authentication.Service", FtpListOption.Recursive);
                    ftpClient.UploadDirectory(authenticationServiceFolder.FullName, "/home/vafnir/Spravy.Authentication.Service");

                    using var commandAuthenticationService =
                        sshClient.RunCommand(
                            $"echo {SshPassword} | sudo systemctl restart spravy.authentication.service"
                        );
                    
                    var toDoServiceFolder = PublishProject("Spravy.ToDo.Service");
                    ftpClient.DeleteDirectory("/home/vafnir/Spravy.ToDo.Service", FtpListOption.Recursive);
                    ftpClient.UploadDirectory(toDoServiceFolder.FullName, "/home/vafnir/Spravy.ToDo.Service");

                    using var commandToDoService =
                        sshClient.RunCommand(
                            $"echo {SshPassword} | sudo systemctl restart spravy.todo.service"
                        );
                    
                    var desktopFolder = PublishProject("Spravy.Ui.Desktop");
                    var desktopAppFolder = new DirectoryInfo("/home/vafnir/Apps/Spravy.Ui.Desktop");

                    if (desktopAppFolder.Exists)
                    {
                        desktopAppFolder.Delete(true);
                    }

                    CopyDirectory(desktopFolder.FullName, desktopAppFolder.FullName, true);
                    /*var browserFolder = PublishProject("Spravy.Ui.Browser");

                 var keyStoreFile = new FileInfo(Solution.Directory / "sign-key.keystore");

                 if (keyStoreFile.Exists)
                 {
                     keyStoreFile.Delete();
                 }

                 await Cli.Wrap("keytool")
                     .WithWorkingDirectory(Solution.Directory)
                     .WithArguments(
                         $"-genkey -v -keystore sign-key.keystore -alias spravy -keyalg RSA -keysize 2048 -validity 10000 -dname \"CN=Serhii Maksymov, OU=Serhii Maksymov FOP, O=Serhii Maksymov FOP, L=Kharkiv, S=Kharkiv State, C=Ukraine\" -storepass {AndroidSigningStorePass}"
                     )
                     .ExecuteAsync();

                 var androidFolder = PublishProject("Spravy.Ui.Android", setting => setting
                     .AddProperty("AndroidKeyStore", "true")
                     .AddProperty("AndroidSigningKeyStore", keyStoreFile.FullName)
                     .AddProperty("AndroidSigningKeyAlias", "spravy")
                     .AddProperty("AndroidSigningKeyPass", AndroidSigningKeyPass)
                     .AddProperty("AndroidSigningStorePass", AndroidSigningStorePass)
                 );*/
                }
            );

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
        var publishFolder = new DirectoryInfo(Path.Combine("/", "tmp", "Spravy", "publish", name));

        if (publishFolder.Exists)
        {
            publishFolder.Delete(true);
        }

        if (!publishFolder.Exists)
        {
            publishFolder.Create();
        }

        var project = Solution.Projects.Single(x => x.Name == name);


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
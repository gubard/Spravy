using _build.Extensions;
using _build.Models;
using CliWrap;
using Nuke.Common.Tools.DotNet;
using Serilog;

namespace _build.Services;

public class DesktopProjectBuilder : UiProjectBuilder<DesktopProjectBuilderOptions>
{
    public DesktopProjectBuilder(SpravyVersion version, DesktopProjectBuilderOptions desktopOptions) : base(
        desktopOptions,
        version
    )
    {
    }

    public void Publish()
    {
        foreach (var runtime in Options.Runtimes.Span)
        {
            var publishFolder = Options.PublishFolder.Combine(runtime.Name);

            DotNetTasks.DotNetPublish(
                setting => setting.SetConfiguration(Options.Configuration)
                   .SetProject(Options.CsprojFile.FullName)
                   .SetOutput(publishFolder.FullName)
                   .EnableNoBuild()
                   .EnableNoRestore()
                   .SetRuntime(runtime.Name)
            );

            if (!runtime.Name.StartsWith("win"))
            {
                continue;
            }

            Cli
               .Wrap("wix")
               .WithWorkingDirectory(publishFolder.FullName)
               .WithArguments(
                    $"build .\\build.wxs -pdbtype none -d Version={version}"
                )
               .ExecuteAsync()
               .GetAwaiter()
               .GetResult();
        }

        var appFolder = Options.GetAppFolder();
        using var ftpClient = Options.CreateFtpClient();
        ftpClient.Connect();
        ftpClient.DeleteIfExistsFolder(appFolder);
        ftpClient.CreateIfNotExistsFolder(Options.GetAppsFolder());
        Log.Information("Upload {LocalFolder} {RemoteFolder}", Options.PublishFolder, appFolder);
        ftpClient.UploadDirectory(Options.PublishFolder.FullName, appFolder.FullName);

        foreach (var publishServer in Options.PublishServers)
        {
            using var sshClient = publishServer.Value.CreateSshClient();
            sshClient.Connect();

            sshClient.SafeRun(
                $"powershell -Command \"rm -Force -Recurse Spravy;git clone https://github.com/gubard/Spravy.git;cd Spravy;nuke --build-desktop --configuration Release --runtime {publishServer.Key} --version {version} --ftp-host {Options.FtpHost} --ftp-user {Options.FtpUser} --ftp-password {Options.FtpPassword} --ftp-user {Options.FtpUser} --domain {Options.Domain}\"",
                true
            );
        }
    }
}
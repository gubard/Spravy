using _build.Extensions;
using _build.Models;
using Nuke.Common.Tools.DotNet;

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
            DotNetTasks.DotNetPublish(
                setting => setting.SetConfiguration(Options.Configuration)
                   .SetProject(Options.CsprojFile.FullName)
                   .SetOutput(Options.PublishFolder.Combine(runtime.Name).FullName)
                   .EnableNoBuild()
                   .EnableNoRestore()
                   .SetRuntime(runtime.Name)
            );
        }

        using var ftpClient = Options.CreateFtpClient();
        ftpClient.Connect();
        ftpClient.DeleteIfExistsFolder(Options.GetAppFolder());
        ftpClient.CreateIfNotExistsFolder(Options.GetAppsFolder());
        ftpClient.UploadDirectory(Options.PublishFolder.FullName, Options.GetAppFolder().FullName);

        foreach (var publishServer in Options.PublishServers)
        {
            using var sshClient = publishServer.Value.CreateSshClient();
            sshClient.Connect();
            sshClient.SafeRun($"powershell -Command \"rm -Force -Recurse Spravy;git clone https://github.com/gubard/Spravy.git;cd Spravy;nuke --build-desktop --configuration Release --runtime {publishServer.Key} --version {version} --ftp-host {Options.FtpHost} --ftp-user {Options.FtpUser} --ftp-password {Options.FtpPassword} --ftp-user {Options.FtpUser} --domain {Options.Domain}\"", true);
        }
    }
}
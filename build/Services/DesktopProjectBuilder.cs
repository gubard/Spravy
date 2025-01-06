using _build.Extensions;
using _build.Models;
using CliWrap;
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
            var publishFolder = Options.PublishFolder.Combine(runtime.Name);

            DotNetTasks.DotNetPublish(
                setting => setting.SetConfiguration(Options.Configuration)
                   .SetProject(Options.CsprojFile.FullName)
                   .SetOutput(publishFolder.FullName)
                   .EnableNoBuild()
                   .EnableNoRestore()
                   .SetRuntime(runtime.Name)
            );

            if (runtime.Name.StartsWith("win"))
            {
                Cli
                   .Wrap("cmd")
                   .WithWorkingDirectory(publishFolder.FullName)
                   .WithArguments(
                        $"wix build .\\build.wxs -pdbtype none -d Version={version}"
                    )
                   .ExecuteAsync()
                   .GetAwaiter()
                   .GetResult();

                foreach (var file in publishFolder.GetFiles())
                {
                    var extension = file.GetFileNameWithoutExtension().ToUpper();

                    if (extension == ".MSI")
                    {
                        continue;
                    }

                    file.Delete();
                }
            }
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

            sshClient.SafeRun(
                $"powershell -Command \"rm -Force -Recurse Spravy;git clone https://github.com/gubard/Spravy.git;cd Spravy;nuke --build-desktop --configuration Release --runtime {publishServer.Key} --version {version} --ftp-host {Options.FtpHost} --ftp-user {Options.FtpUser} --ftp-password {Options.FtpPassword} --ftp-user {Options.FtpUser} --domain {Options.Domain}\"",
                true
            );
        }
    }
}
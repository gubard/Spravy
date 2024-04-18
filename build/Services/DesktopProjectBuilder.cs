using _build.Extensions;
using _build.Models;
using Nuke.Common.Tools.DotNet;

namespace _build.Services;

public class DesktopProjectBuilder : UiProjectBuilder<DesktopProjectBuilderOptions>
{
    public DesktopProjectBuilder(
        VersionService versionService,
        DesktopProjectBuilderOptions desktopOptions
    ) : base(desktopOptions, versionService)
    {
    }

    public void Publish()
    {
        if (Options.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(Options.Configuration)
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
                DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(Options.Configuration)
                    .SetProject(Options.CsprojFile.FullName)
                    .SetOutput(Options.PublishFolder.Combine(runtime.Name).FullName)
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .SetRuntime(runtime.Name)
                );
            }
        }

        using var ftpClient = Options.CreateFtpClient();
        ftpClient.Connect();
        ftpClient.DeleteIfExistsFolder(Options.GetAppFolder());
        ftpClient.CreateIfNotExistsFolder(Options.GetAppsFolder());

        ftpClient.UploadDirectory(
            Options.PublishFolder.FullName,
            Options.GetAppFolder().FullName
        );
    }
}
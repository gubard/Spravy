using _build.Extensions;
using _build.Models;
using Nuke.Common.Tools.DotNet;

namespace _build.Services;

public class DesktopProjectBuilder : UiProjectBuilder
{
    readonly DesktopProjectBuilderOptions desktopOptions;

    public DesktopProjectBuilder(
        VersionService versionService,
        DesktopProjectBuilderOptions desktopOptions
    ) : base(desktopOptions, versionService)
    {
        this.desktopOptions = desktopOptions;
    }

    public void Publish()
    {
        if (options.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(options.Configuration)
                .SetProject(options.CsprojFile.FullName)
                .SetOutput(desktopOptions.PublishFolder.FullName)
                .EnableNoBuild()
                .EnableNoRestore()
            );
        }
        else
        {
            foreach (var runtime in options.Runtimes.Span)
            {
                DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(options.Configuration)
                    .SetProject(options.CsprojFile.FullName)
                    .SetOutput(desktopOptions.PublishFolder.Combine(runtime.Name).FullName)
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .SetRuntime(runtime.Name)
                );
            }
        }

        using var ftpClient = desktopOptions.CreateFtpClient();
        ftpClient.Connect();
        ftpClient.DeleteIfExistsFolder(desktopOptions.GetAppFolder());
        ftpClient.CreateIfNotExistsFolder(desktopOptions.GetAppsFolder());

        ftpClient.UploadDirectory(
            desktopOptions.PublishFolder.FullName,
            desktopOptions.GetAppFolder().FullName
        );
    }
}
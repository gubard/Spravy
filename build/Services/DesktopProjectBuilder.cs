using _build.Extensions;
using _build.Models;
using Nuke.Common.Tools.DotNet;

namespace _build.Services;

public class DesktopProjectBuilder : UiProjectBuilder
{
    readonly DesktopProjectBuilderOptions desktopProjectBuilderOptions;

    public DesktopProjectBuilder(
        VersionService versionService,
        DesktopProjectBuilderOptions desktopProjectBuilderOptions
    )
        : base(desktopProjectBuilderOptions, versionService)
    {
        this.desktopProjectBuilderOptions = desktopProjectBuilderOptions;
    }

    public void Publish()
    {
        using var ftpClient = desktopProjectBuilderOptions.CreateFtpClient();
        ftpClient.Connect();

        foreach (var runtime in projectBuilderOptions.Runtimes.Span)
        {
            var output = desktopProjectBuilderOptions.PublishFolder.Combine(runtime.Name);
            output.DeleteIfExits();

            DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(projectBuilderOptions.Configuration)
                .SetProject(projectBuilderOptions.CsprojFile.FullName)
                .SetOutput(output.FullName)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetRuntime(runtime.Name)
            );

            ftpClient.DeleteIfExistsFolder($"/home/{desktopProjectBuilderOptions.FtpUser}/Apps/Spravy.Ui.Desktop"
                .ToFolder()
                .Combine(runtime.Name)
            );

            ftpClient.CreateIfNotExistsDirectory($"/home/{desktopProjectBuilderOptions.FtpUser}/Apps/Spravy.Ui.Desktop"
                .ToFolder()
                .Combine(runtime.Name)
            );

            ftpClient.UploadDirectory(output.FullName,
                $"/home/{desktopProjectBuilderOptions.FtpUser}/Apps/Spravy.Ui.Desktop/{runtime.Name}"
            );
        }
    }
}
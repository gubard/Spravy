using System.IO;
using System.Linq;
using _build.Extensions;
using _build.Helpers;
using _build.Interfaces;
using _build.Models;
using _build.Services;
using Nuke.Common;
using Nuke.Common.ProjectModel;

namespace _build.Builds;

class DesktopPublishSingleBuild : NukeBuild
{
    [Solution]
    readonly Solution Solution;

    [Parameter]
    readonly string Version;

    [Parameter]
    readonly string Runtime;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter]
    readonly string FtpHost;

    [Parameter]
    readonly string FtpPassword;

    [Parameter]
    readonly string FtpUser;

    [Parameter]
    readonly string Domain;

    [Parameter]
    readonly string PublishFolder;

    DesktopProjectBuilder Project;

    public static int Execute() => Execute<DesktopPublishSingleBuild>(x => x.Publish);

    Target SetupAppSettings =>
        _ => _.Executes(
            () =>
            {
                var csprojFile = Solution.AllProjects
                   .Select(x => new FileInfo(x.Path))
                   .Single(x => x.GetFileNameWithoutExtension().EndsWith(".Desktop"));

                SpravyVersion.TryParse(Version, out var version);

                Project = new(
                    version,
                    new(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        BuildHelper.Ports,
                        new[]
                        {
                            new Runtime(Runtime),
                        },
                        Configuration,
                        Domain,
                        FtpHost,
                        FtpUser,
                        FtpPassword,
                        new DirectoryInfo(PublishFolder).Combine(csprojFile.GetFileNameWithoutExtension()),
                        new()
                    )
                );

                BuildHelper.SetupAppSettings(
                    new IProjectBuilder[]
                    {
                        Project,
                    }
                );
            }
        );

    Target Publish =>
        _ => _.Executes(
            () => BuildHelper.PublishDesktop(
                new IProjectBuilder[]
                {
                    Project,
                }
            )
        );
}
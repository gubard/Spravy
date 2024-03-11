using System.Linq;
using _build.Interfaces;
using _build.Models;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;

namespace _build.Services;

public abstract class ProjectBuilder : IProjectBuilder
{
    protected readonly ProjectBuilderOptions projectBuilderOptions;
    protected readonly VersionService versionService;

    protected ProjectBuilder(ProjectBuilderOptions projectBuilderOptions, VersionService versionService)
    {
        this.projectBuilderOptions = projectBuilderOptions;
        this.versionService = versionService;
    }

    public abstract void Setup(string host);

    public void Clean()
    {
        if (projectBuilderOptions.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetClean(setting => setting.SetProject(projectBuilderOptions.CsprojFile.FullName)
                .SetConfiguration(projectBuilderOptions.Configuration)
            );
        }
        else
        {
            foreach (var runtime in projectBuilderOptions.Runtimes.Span)
            {
                DotNetTasks.DotNetClean(setting => setting.SetProject(projectBuilderOptions.CsprojFile.FullName)
                    .SetConfiguration(projectBuilderOptions.Configuration)
                    .SetRuntime(runtime.Name)
                );
            }
        }
    }

    public void Restore()
    {
        DotNetTasks.DotNetRestore(setting =>
            {
                var result = setting.SetProjectFile(projectBuilderOptions.CsprojFile.FullName);

                if (!projectBuilderOptions.Runtimes.IsEmpty)
                {
                    result = result.SetRuntime(projectBuilderOptions.Runtimes.ToArray()
                        .Select(x => x.Name)
                        .JoinSemicolon()
                    );
                }

                return result;
            }
        );
    }

    public virtual void Compile()
    {
        DotNetTasks.DotNetBuild(setting =>
            {
                var result = setting.SetProjectFile(projectBuilderOptions.CsprojFile.FullName)
                    .EnableNoRestore()
                    .SetConfiguration(projectBuilderOptions.Configuration)
                    .AddProperty("Version", versionService.Version.ToString());

                if (!projectBuilderOptions.Runtimes.IsEmpty)
                {
                    result = result.SetRuntime(projectBuilderOptions.Runtimes.ToArray()
                        .Select(x => x.Name)
                        .JoinSemicolon()
                    );
                }

                return result;
            }
        );
    }
}
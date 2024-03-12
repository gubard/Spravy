using _build.Interfaces;
using _build.Models;
using Nuke.Common.Tools.DotNet;

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

    public abstract void Setup();

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
        if (projectBuilderOptions.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetRestore(setting => setting.SetProjectFile(projectBuilderOptions.CsprojFile.FullName));
        }
        else
        {
            foreach (var runtime in projectBuilderOptions.Runtimes.Span)
            {
                DotNetTasks.DotNetRestore(setting =>
                    setting.SetProjectFile(projectBuilderOptions.CsprojFile.FullName).SetRuntime(runtime.Name)
                );
            }
        }
    }

    public virtual void Compile()
    {
        if (projectBuilderOptions.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetBuild(setting => setting.SetProjectFile(projectBuilderOptions.CsprojFile.FullName)
                .EnableNoRestore()
                .SetConfiguration(projectBuilderOptions.Configuration)
                .AddProperty("Version", versionService.Version.ToString())
            );
        }
        else
        {
            foreach (var runtime in projectBuilderOptions.Runtimes.Span)
            {
                DotNetTasks.DotNetBuild(setting => setting.SetProjectFile(projectBuilderOptions.CsprojFile.FullName)
                    .EnableNoRestore()
                    .SetConfiguration(projectBuilderOptions.Configuration)
                    .AddProperty("Version", versionService.Version.ToString())
                    .SetRuntime(runtime.Name)
                );
            }
        }
    }
}
using _build.Interfaces;
using _build.Models;
using Nuke.Common.Tools.DotNet;

namespace _build.Services;

public abstract class ProjectBuilder : IProjectBuilder
{
    protected readonly ProjectBuilderOptions options;
    protected readonly VersionService versionService;

    protected ProjectBuilder(ProjectBuilderOptions options, VersionService versionService)
    {
        this.options = options;
        this.versionService = versionService;
    }

    public abstract void Setup();

    public void Clean()
    {
        if (options.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetClean(setting => setting.SetProject(options.CsprojFile.FullName)
                .SetConfiguration(options.Configuration)
            );
        }
        else
        {
            foreach (var runtime in options.Runtimes.Span)
            {
                DotNetTasks.DotNetClean(setting => setting.SetProject(options.CsprojFile.FullName)
                    .SetConfiguration(options.Configuration)
                    .SetRuntime(runtime.Name)
                );
            }
        }
    }

    public void Restore()
    {
        if (options.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetRestore(setting => setting.SetProjectFile(options.CsprojFile.FullName));
        }
        else
        {
            foreach (var runtime in options.Runtimes.Span)
            {
                DotNetTasks.DotNetRestore(setting =>
                    setting.SetProjectFile(options.CsprojFile.FullName).SetRuntime(runtime.Name)
                );
            }
        }
    }

    public virtual void Compile()
    {
        if (options.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetBuild(setting => setting.SetProjectFile(options.CsprojFile.FullName)
                .EnableNoRestore()
                .SetConfiguration(options.Configuration)
                .AddProperty("Version", versionService.Version.ToString())
            );
        }
        else
        {
            foreach (var runtime in options.Runtimes.Span)
            {
                DotNetTasks.DotNetBuild(setting => setting.SetProjectFile(options.CsprojFile.FullName)
                    .EnableNoRestore()
                    .SetConfiguration(options.Configuration)
                    .AddProperty("Version", versionService.Version.ToString())
                    .SetRuntime(runtime.Name)
                );
            }
        }
    }
}
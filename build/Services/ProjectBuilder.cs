using System.IO;
using _build.Extensions;
using _build.Interfaces;
using _build.Models;
using Nuke.Common.Tools.DotNet;

namespace _build.Services;

public abstract class ProjectBuilder<TOptions> : IProjectBuilder where TOptions : ProjectBuilderOptions
{
    protected readonly VersionService versionService;

    protected ProjectBuilder(TOptions options, VersionService versionService)
    {
        Options = options;
        this.versionService = versionService;
    }

    public TOptions Options { get; }

    public abstract void Setup();

    public void Clean()
    {
        if (Options.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetClean(setting => setting.SetProject(Options.CsprojFile.FullName)
               .SetConfiguration(Options.Configuration));
        }
        else
        {
            foreach (var runtime in Options.Runtimes.Span)
            {
                DotNetTasks.DotNetClean(setting => setting.SetProject(Options.CsprojFile.FullName)
                   .SetConfiguration(Options.Configuration)
                   .SetRuntime(runtime.Name));
            }
        }

        GetBinFolder().DeleteIfExits();
        GetObjFolder().DeleteIfExits();
    }

    public virtual void Restore()
    {
        if (Options.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetRestore(setting => setting.SetProjectFile(Options.CsprojFile.FullName));
        }
        else
        {
            foreach (var runtime in Options.Runtimes.Span)
            {
                DotNetTasks.DotNetRestore(setting =>
                    setting.SetProjectFile(Options.CsprojFile.FullName).SetRuntime(runtime.Name));
            }
        }
    }

    public virtual void Compile()
    {
        if (Options.Runtimes.IsEmpty)
        {
            DotNetTasks.DotNetBuild(setting => setting.SetProjectFile(Options.CsprojFile.FullName)
               .EnableNoRestore()
               .SetConfiguration(Options.Configuration)
               .AddProperty("Version", versionService.Version.ToString()));
        }
        else
        {
            foreach (var runtime in Options.Runtimes.Span)
            {
                DotNetTasks.DotNetBuild(setting => setting.SetProjectFile(Options.CsprojFile.FullName)
                   .EnableNoRestore()
                   .SetConfiguration(Options.Configuration)
                   .AddProperty("Version", versionService.Version.ToString())
                   .SetRuntime(runtime.Name));
            }
        }
    }

    DirectoryInfo GetBinFolder() => Options.CsprojFile.Directory.Combine("bin");

    DirectoryInfo GetObjFolder() => Options.CsprojFile.Directory.Combine("obj");
}
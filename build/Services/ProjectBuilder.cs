using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _build.Extensions;
using _build.Interfaces;
using _build.Models;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;

namespace _build.Services;

public abstract class ProjectBuilder : IProjectBuilder
{
    protected readonly FileInfo csprojFile;
    protected readonly FileInfo appSettingsFile;
    protected readonly IReadOnlyDictionary<string, ushort> hosts;
    protected readonly ReadOnlyMemory<Runtime> runtimes;
    protected readonly string configuration;
    protected readonly VersionService versionService;

    protected ProjectBuilder(
        FileInfo csprojFile,
        IReadOnlyDictionary<string, ushort> hosts,
        IEnumerable<Runtime> runtimes,
        string configuration,
        VersionService versionService
    )
    {
        this.csprojFile = csprojFile;
        this.hosts = hosts;
        this.configuration = configuration;
        this.versionService = versionService;
        appSettingsFile = csprojFile.Directory.ToFile("appsettings.json");
        this.runtimes = runtimes.ToArray();
    }

    public abstract void Setup(string host);

    public void Clean()
    {
        DotNetTasks.DotNetClean(setting =>
            {
                var result = setting.SetProject(csprojFile.FullName)
                    .SetConfiguration(configuration);

                if (runtimes.IsEmpty)
                {
                    result = result.SetRuntime(runtimes.ToArray().Select(x => x.Name).JoinSemicolon());
                }

                return result;
            }
        );
    }

    public void Restore()
    {
        DotNetTasks.DotNetRestore(setting =>
            {
                var result = setting.SetProjectFile(csprojFile.FullName);

                if (runtimes.IsEmpty)
                {
                    result = result.SetRuntime(runtimes.ToArray().Select(x => x.Name).JoinSemicolon());
                }

                return result;
            }
        );
    }

    public virtual void Compile()
    {
        DotNetTasks.DotNetBuild(setting =>
            {
                var result = setting.SetProjectFile(csprojFile.FullName)
                    .EnableNoRestore()
                    .SetConfiguration(configuration)
                    .AddProperty("Version", versionService.Version.ToString());

                if (runtimes.IsEmpty)
                {
                    result = result.SetRuntime(runtimes.ToArray().Select(x => x.Name).JoinSemicolon());
                }

                return result;
            }
        );
    }
}
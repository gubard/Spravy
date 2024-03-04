using System.Collections.Generic;
using System.IO;
using _build.Extensions;
using _build.Interfaces;
using Nuke.Common.Tools.DotNet;

namespace _build.Services;

public abstract class ProjectBuilder : IProjectBuilder
{
    protected readonly FileInfo csprojFile;
    protected readonly FileInfo appSettingsFile;
    protected readonly IReadOnlyDictionary<string, ushort> hosts;

    protected ProjectBuilder(FileInfo csprojFile, IReadOnlyDictionary<string, ushort> hosts)
    {
        this.csprojFile = csprojFile;
        this.hosts = hosts;
        appSettingsFile = csprojFile.Directory.ToFile("appsettings.json");
    }

    public abstract void Setup(string host);

    public void Clean(string configuration)
    {
        DotNetTasks.DotNetClean(setting => setting.SetProject(csprojFile.FullName).SetConfiguration(configuration));
    }
}
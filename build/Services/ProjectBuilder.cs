using System.IO;
using _build.Extensions;
using _build.Interfaces;

namespace _build.Services;

public abstract class ProjectBuilder : IProjectBuilder
{
    protected readonly FileInfo csprojFile;
    protected readonly FileInfo appSettingsFile;

    protected ProjectBuilder(FileInfo csprojFile)
    {
        this.csprojFile = csprojFile;
        appSettingsFile = csprojFile.Directory.ToFile("appsettings.json");
    }

    public void Setup(string host)
    {
        
    }
}
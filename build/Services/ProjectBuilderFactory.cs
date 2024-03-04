using System.Collections.Generic;
using System.IO;
using _build.Extensions;
using _build.Interfaces;

namespace _build.Services;

public class ProjectBuilderFactory
{
    readonly IReadOnlyDictionary<string, ushort> servicePorts;

    public ProjectBuilderFactory(IReadOnlyDictionary<string, ushort> servicePorts)
    {
        this.servicePorts = servicePorts;
    }

    public IEnumerable<IProjectBuilder> Create(IEnumerable<FileInfo> csprojFiles)
    {
        foreach (var csprojFile in csprojFiles)
        {
            var fileName = csprojFile.GetFileNameWithoutExtension();

            if (fileName == "Spravy.Service")
            {
                continue;
            }

            if (fileName.EndsWith(".Service"))
            {
                yield return new ServiceProjectBuilder(csprojFile, servicePorts[fileName]);
            }

            if (fileName.EndsWith(".Android"))
            {
                yield return new AndroidProjectBuilder(csprojFile);
            }

            if (fileName.EndsWith(".Browser"))
            {
                yield return new BrowserProjectBuilder(csprojFile);
            }
            
            if (fileName.EndsWith(".Desktop"))
            {
                yield return new DesktopProjectBuilder(csprojFile);
            }
        }
    }
}
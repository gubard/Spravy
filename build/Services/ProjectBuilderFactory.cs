using System.Collections.Generic;
using System.IO;
using _build.Extensions;
using _build.Interfaces;
using _build.Models;

namespace _build.Services;

public class ProjectBuilderFactory
{
    readonly IReadOnlyDictionary<string, ushort> servicePorts;
    readonly string token;
    readonly string emailPassword;
    readonly string configuration;
    readonly VersionService versionService;

    public ProjectBuilderFactory(
        IReadOnlyDictionary<string, ushort> servicePorts,
        string token,
        string emailPassword,
        string configuration,
        VersionService versionService
    )
    {
        this.servicePorts = servicePorts;
        this.token = token;
        this.emailPassword = emailPassword;
        this.configuration = configuration;
        this.versionService = versionService;
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
                yield return new ServiceProjectBuilder(
                    csprojFile,
                    servicePorts[fileName.GetGrpcServiceName()],
                    token,
                    servicePorts,
                    emailPassword,
                    new[]
                    {
                        Runtime.LinuxX64
                    },
                    configuration,
                    versionService
                );
            }

            if (fileName.EndsWith(".Android"))
            {
                yield return new AndroidProjectBuilder(csprojFile, servicePorts, configuration, versionService);
            }

            if (fileName.EndsWith(".Browser"))
            {
                yield return new BrowserProjectBuilder(csprojFile, servicePorts, configuration, versionService);
            }

            if (fileName.EndsWith(".Desktop"))
            {
                yield return new DesktopProjectBuilder(csprojFile, servicePorts, configuration, versionService);
            }
        }
    }
}
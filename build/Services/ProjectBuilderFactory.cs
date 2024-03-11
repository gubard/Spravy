using System.Collections.Generic;
using System.IO;
using System.Linq;
using _build.Extensions;
using _build.Interfaces;
using _build.Models;

namespace _build.Services;

public class ProjectBuilderFactory
{
    readonly string configuration;
    readonly string token;
    readonly string emailPassword;
    readonly IReadOnlyDictionary<string, ushort> ports;
    readonly AndroidProjectBuilderOptions androidProjectBuilderOptions;
    readonly VersionService versionService;

    public ProjectBuilderFactory(
        string configuration,
        string emailPassword,
        string token,
        IReadOnlyDictionary<string, ushort> ports,
        AndroidProjectBuilderOptions androidProjectBuilderOptions,
        VersionService versionService
    )
    {
        this.versionService = versionService;
        this.emailPassword = emailPassword;
        this.token = token;
        this.configuration = configuration;
        this.ports = ports;
        this.androidProjectBuilderOptions = androidProjectBuilderOptions;
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
                    new ProjectBuilderOptions(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        ports,
                        new[]
                        {
                            Runtime.LinuxX64,
                        },
                        configuration
                    ),
                    versionService,
                    new ServiceProjectBuilderOptions(ports[csprojFile.GetGrpcServiceName()], token, emailPassword)
                );
            }

            if (fileName.EndsWith(".Android"))
            {
                yield return new AndroidProjectBuilder(
                    new ProjectBuilderOptions(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        ports,
                        Enumerable.Empty<Runtime>(),
                        configuration
                    ),
                    versionService,
                    androidProjectBuilderOptions
                );
            }

            if (fileName.EndsWith(".Browser"))
            {
                yield return new BrowserProjectBuilder(
                    new ProjectBuilderOptions(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        ports,
                        new[]
                        {
                            Runtime.BrowserWasm,
                        },
                        configuration
                    ),
                    versionService
                );
            }

            if (fileName.EndsWith(".Desktop"))
            {
                yield return new DesktopProjectBuilder(
                    new ProjectBuilderOptions(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        ports,
                        new[]
                        {
                            Runtime.LinuxX64, Runtime.WinX64,
                        },
                        configuration
                    ), versionService
                );
            }
        }
    }
}
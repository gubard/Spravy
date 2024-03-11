using System.Collections.Generic;
using System.IO;
using _build.Models;

namespace _build.Services;

public class DesktopProjectBuilder : UiProjectBuilder
{
    public DesktopProjectBuilder(
        FileInfo csprojFile,
        IReadOnlyDictionary<string, ushort> hosts,
        string configuration,
        VersionService versionService
    )
        : base(
            csprojFile,
            hosts, new[]
            {
                Runtime.LinuxX64, Runtime.WinX64,
            },
            configuration,
            versionService
        )
    {
    }
}
using System.Collections.Generic;
using System.IO;

namespace _build.Services;

public class DesktopProjectBuilder : UiProjectBuilder
{
    public DesktopProjectBuilder(FileInfo csprojFile, IReadOnlyDictionary<string, ushort> hosts) : base(csprojFile, hosts)
    {
    }
}
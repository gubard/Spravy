using System.Collections.Generic;
using System.IO;

namespace _build.Services;

public class AndroidProjectBuilder : UiProjectBuilder
{
    public AndroidProjectBuilder(FileInfo csprojFile, IReadOnlyDictionary<string, ushort> hosts) : base(csprojFile, hosts)
    {
    }
}
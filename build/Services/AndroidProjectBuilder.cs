using System.Collections.Generic;
using System.IO;
using System.Linq;
using _build.Models;

namespace _build.Services;

public class AndroidProjectBuilder : UiProjectBuilder
{
    public AndroidProjectBuilder(FileInfo csprojFile, IReadOnlyDictionary<string, ushort> hosts) 
        : base(csprojFile, hosts, Enumerable.Empty<Runtime>()
    )
    {
    }
}
using System.Collections.Generic;
using System.IO;
using _build.Models;

namespace _build.Services;

public class BrowserProjectBuilder : UiProjectBuilder
{
    public BrowserProjectBuilder(
        FileInfo csprojFile,
        IReadOnlyDictionary<string, ushort> hosts
    ) : base(
        csprojFile,
        hosts,
        new[]
        {
            Runtime.BrowserWasm,
        }
    )
    {
    }
}
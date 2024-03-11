using System.Collections.Generic;
using System.IO;
using System.Text;
using _build.Extensions;
using _build.Models;

namespace _build.Services;

public abstract class UiProjectBuilder : ProjectBuilder
{
    protected UiProjectBuilder(
        FileInfo csprojFile,
        IReadOnlyDictionary<string, ushort> hosts,
        IEnumerable<Runtime> runtimes
    ) : base(csprojFile, hosts, runtimes)
    {
    }

    public override void Setup(string host)
    {
        var jsonDocument = appSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        stream.SetAppSettingsStream(host, jsonDocument, hosts);
        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(appSettingsFile.FullName, jsonData);
    }
}
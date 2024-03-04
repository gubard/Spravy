using System.Collections.Generic;
using System.IO;
using System.Text;
using _build.Extensions;

namespace _build.Services;

public abstract class UiProjectBuilder : ProjectBuilder
{
    protected UiProjectBuilder(FileInfo csprojFile, IReadOnlyDictionary<string, ushort> hosts) : base(csprojFile, hosts)
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
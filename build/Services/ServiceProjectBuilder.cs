using System.Collections.Generic;
using System.IO;
using System.Text;
using _build.Extensions;
using Serilog;

namespace _build.Services;

public class ServiceProjectBuilder : ProjectBuilder
{
    readonly ushort port;
    readonly string token;
    readonly string emailPassword;

    public ServiceProjectBuilder(
        FileInfo csprojFile,
        ushort port,
        string token,
        IReadOnlyDictionary<string, ushort> hosts,
        string emailPassword
    ) : base(csprojFile, hosts)
    {
        this.port = port;
        this.token = token;
        this.emailPassword = emailPassword;
    }

    public override void Setup(string host)
    {
        var jsonDocument = appSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        Log.Logger.Information("Set app settings {File}", appSettingsFile);
        stream.SetAppSettingsStream(jsonDocument, host, hosts, token, port, emailPassword);
        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(appSettingsFile.FullName, jsonData);
    }
}
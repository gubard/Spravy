using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace _build.Extensions;

public static class StreamExtension
{
    public static void SetAppSettingsStream(
        this Stream stream,
        JsonDocument jsonDocument,
        string domain,
        IReadOnlyDictionary<string, ushort> hosts,
        string token,
        uint port,
        string emailPassword
    )
    {
        var jsonWriterOptions = new JsonWriterOptions { Indented = true };

        using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);
        writer.WriteStartObject();

        foreach (var obj in jsonDocument.RootElement.EnumerateObject())
        {
            if (writer.SetKestrel(obj, port, domain))
            {
                continue;
            }

            if (writer.SetServices(obj, domain, hosts, token))
            {
                continue;
            }

            if (writer.SetDomain(obj, domain))
            {
                continue;
            }

            if (writer.SetEmailService(obj, emailPassword))
            {
                continue;
            }

            if (writer.SetSerilog(obj))
            {
                continue;
            }

            obj.WriteTo(writer);
        }

        writer.WriteEndObject();
    }

    public static void SetAppSettingsStream(
        this Stream stream,
        string domain,
        JsonDocument jsonDocument,
        IReadOnlyDictionary<string, ushort> hosts
    )
    {
        var jsonWriterOptions = new JsonWriterOptions { Indented = true };

        using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);
        writer.WriteStartObject();

        foreach (var obj in jsonDocument.RootElement.EnumerateObject())
        {
            if (writer.SetServices(obj, domain, hosts, string.Empty))
            {
                continue;
            }

            if (writer.SetSerilog(obj))
            {
                continue;
            }

            obj.WriteTo(writer);
        }

        writer.WriteEndObject();
    }
}

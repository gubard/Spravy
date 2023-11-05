using System.Collections.Generic;
using System.Text.Json;
using Serilog;

namespace _build.Extensions;

public static class Utf8JsonWriterExtension
{
    public static bool SetServices(
        this Utf8JsonWriter writer,
        JsonProperty property,
        Dictionary<string, string> hosts,
        string token
    )
    {
        if (!hosts.TryGetValue(property.Name, out var host))
        {
            return false;
        }

        writer.WritePropertyName(property.Name);
        writer.WriteStartObject();

        foreach (var obj in property.Value.EnumerateObject())
        {
            if (obj.Name == "Host")
            {
                writer.WritePropertyName("Host");
                writer.WriteStringValue(host);
            }
            else if (obj.Name == "Token")
            {
                writer.WritePropertyName("Token");
                writer.WriteStringValue(token);
            }
            else
            {
                obj.WriteTo(writer);
            }
        }

        writer.WriteEndObject();
        Log.Information("Setup service {ServiceName}", property.Name);
        Log.Information("Set host {Host}", host);

        return true;
    }

    public static bool SetUrls(this Utf8JsonWriter writer, JsonProperty property, string urls)
    {
        if (property.Name != "Urls")
        {
            return false;
        }

        writer.WritePropertyName("Urls");
        writer.WriteStringValue(urls);
        Log.Information("Set URLs {Urls}", urls);

        return true;
    }

    public static bool SetSerilog(this Utf8JsonWriter writer, JsonProperty property)
    {
        if (property.Name != "Serilog")
        {
            return false;
        }

        writer.WritePropertyName("Serilog");
        writer.WriteStartObject();

        foreach (var ser in property.Value.EnumerateObject())
        {
            if (ser.Name == "MinimumLevel")
            {
                writer.WritePropertyName("MinimumLevel");
                writer.WriteStartObject();
                writer.WritePropertyName("Default");
                writer.WriteStringValue("Information");
                writer.WritePropertyName("Override");
                writer.WriteStartObject();
                writer.WritePropertyName("Microsoft");
                writer.WriteStringValue("Warning");
                writer.WriteEndObject();
                writer.WriteEndObject();

                continue;
            }

            ser.WriteTo(writer);
        }

        writer.WriteEndObject();

        return true;
    }
}
using System.Collections.Generic;
using System.Text.Json;
using Serilog;

namespace _build.Extensions;

public static class Utf8JsonWriterExtension
{
    public static bool SetKestrel(
        this Utf8JsonWriter writer,
        JsonProperty property,
        uint port
    )
    {
        if (property.Name != "Kestrel")
        {
            return false;
        }

        foreach (var obj in property.Value.EnumerateObject())
        {
            obj.WriteTo(writer);
        }

        writer.WritePropertyName("EndPoints");
        writer.WriteStartObject();
        writer.WritePropertyName("HttpsFromPem");
        writer.WriteStartObject();
        writer.WritePropertyName("Url");
        writer.WriteStringValue($"https://0.0.0.0:{port}");
        writer.WritePropertyName("Certificate");
        writer.WriteStartObject();
        writer.WritePropertyName("Path");
        writer.WriteStringValue("/etc/letsencrypt/live/spravy.com.ua/fullchain.pem");
        writer.WritePropertyName("KeyPath");
        writer.WriteStringValue("/etc/letsencrypt/live/spravy.com.ua/privkey.pem");
        writer.WriteEndObject();
        writer.WriteEndObject();
        writer.WriteEndObject();

        return true;
    }

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
            switch (obj.Name)
            {
                case "Host":
                    writer.WritePropertyName("Host");
                    writer.WriteStringValue(host);
                    break;
                case "Token":
                    writer.WritePropertyName("Token");
                    writer.WriteStringValue(token);
                    break;
                case "ChannelCredentialType":
                    writer.WritePropertyName("ChannelCredentialType");
                    writer.WriteStringValue("SecureSsl");
                    break;
                default:
                    obj.WriteTo(writer);
                    break;
            }
        }

        writer.WriteEndObject();
        Log.Information("Setup service {ServiceName}", property.Name);
        Log.Information("Set host {Host}", host);

        return true;
    }

    public static bool SetDomain(this Utf8JsonWriter writer, JsonProperty property, string domain)
    {
        if (property.Name != "Urls")
        {
            return false;
        }

        writer.WritePropertyName("UrlDomain");
        writer.WriteStringValue(domain);
        Log.Information("Set URL domain {Domain}", domain);

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
                writer.WriteStringValue("Information");
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
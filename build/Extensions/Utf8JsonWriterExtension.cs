using System;
using System.Collections.Generic;
using System.Text.Json;
using Serilog;

namespace _build.Extensions;

public static class Utf8JsonWriterExtension
{
    public static void AddObject(this Utf8JsonWriter writer, string name, Action<Utf8JsonWriter> setup)
    {
        writer.WritePropertyName(name);
        writer.WriteStartObject();
        setup.Invoke(writer);
        writer.WriteEndObject();
    }

    public static void AddObject(this Utf8JsonWriter writer, string name, Action setup)
    {
        writer.WritePropertyName(name);
        writer.WriteStartObject();
        setup.Invoke();
        writer.WriteEndObject();
    }

    public static void AddStringValue(this Utf8JsonWriter writer, string name, string value)
    {
        writer.WritePropertyName(name);
        writer.WriteStringValue(value);
    }

    public static bool SetKestrel(
        this Utf8JsonWriter writer,
        JsonProperty property,
        uint port,
        string domain
    )
    {
        if (property.Name != "Kestrel")
        {
            return false;
        }

        writer.AddObject(property.Name, () =>
            {
                writer.AddObject("EndPoints", () =>
                    writer.AddObject("HttpsInlineCertAndKeyFile", () =>
                        {
                            writer.AddStringValue("Url", $"https://0.0.0.0:{port}");
                            Log.Information("Added Url: https://0.0.0.0:{Port}", port);

                            writer.AddObject("Certificate", () =>
                                {
                                    writer.AddStringValue("Path", $"/etc/letsencrypt/live/{domain}/fullchain.pem");
                                    writer.AddStringValue("KeyPath", $"/etc/letsencrypt/live/{domain}/privkey.pem");
                                }
                            );

                            Log.Information("Added Certificate");
                        }
                    )
                );

                foreach (var obj in property.Value.EnumerateObject())
                {
                    if (obj.Name == "EndPoints")
                    {
                        continue;
                    }

                    Log.Information("Added {Object}", obj.Name);
                    obj.WriteTo(writer);
                }
            }
        );

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

        writer.AddObject(property.Name, () =>
            {
                foreach (var obj in property.Value.EnumerateObject())
                {
                    switch (obj.Name)
                    {
                        case "Host":
                            writer.AddStringValue("Host", host);

                            break;
                        case "Token":
                            writer.AddStringValue("Token", token);

                            break;
                        case "ChannelCredentialType":
                            writer.AddStringValue("ChannelCredentialType", "SecureSsl");

                            break;
                        default:
                            obj.WriteTo(writer);

                            break;
                    }
                }
            }
        );

        Log.Information("Setup service {ServiceName}", property.Name);
        Log.Information("Set host {Host}", host);

        return true;
    }

    public static bool SetEmailService(
        this Utf8JsonWriter writer,
        JsonProperty property,
        string host,
        string login,
        string password
    )
    {
        if (property.Name != "EmailService")
        {
            return false;
        }

        writer.AddObject(property.Name, w =>
            {
                w.AddStringValue("Host", host);
                w.AddStringValue("Login", login);
                w.AddStringValue("Password", password);
            }
        );

        return true;
    }

    public static bool SetDomain(this Utf8JsonWriter writer, JsonProperty property, string domain)
    {
        if (property.Name != "Urls")
        {
            return false;
        }

        writer.AddStringValue("UrlDomain", domain);
        Log.Information("Set URL domain {Domain}", domain);

        return true;
    }

    public static bool SetSerilog(this Utf8JsonWriter writer, JsonProperty property)
    {
        if (property.Name != "Serilog")
        {
            return false;
        }

        writer.AddObject("Serilog", () =>
            {
                foreach (var ser in property.Value.EnumerateObject())
                {
                    if (ser.Name == "MinimumLevel")
                    {
                        writer.AddObject("MinimumLevel", () =>
                            {
                                writer.AddStringValue("Default", "Information");
                                writer.AddObject("Override", () => writer.AddStringValue("Microsoft", "Information"));
                            }
                        );

                        continue;
                    }

                    ser.WriteTo(writer);
                }
            }
        );

        return true;
    }
}
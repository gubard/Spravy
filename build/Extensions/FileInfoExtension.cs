using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace _build.Extensions;

public static class FileInfoExtension
{
    public static void SetAppSettingsFile(
        this FileInfo file,
        string urls,
        Dictionary<string, string> hosts,
        string token
    )
    {
        var jsonDocument = file.GetJsonDocument();
        using var stream = new MemoryStream();
        stream.SetAppSettingsStream(jsonDocument, urls, hosts, token);
        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(file.FullName, jsonData);
    }

    public static void SetAppSettingsFile(
        this FileInfo file,
        Dictionary<string, string> hosts,
        string token
    )
    {
        var jsonDocument = file.GetJsonDocument();
        using var stream = new MemoryStream();
        stream.SetAppSettingsStream(jsonDocument, hosts, token);
        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(file.FullName, jsonData);
    }

    public static JsonDocument GetJsonDocument(this FileInfo file)
    {
        return JsonDocument.Parse(file.ReadAllText());
    }

    public static string ReadAllText(this FileInfo file)
    {
        return File.ReadAllText(file.FullName);
    }

    public static void WriteAllText(this FileInfo file, string text)
    {
        File.WriteAllText(file.FullName, text);
    }
}
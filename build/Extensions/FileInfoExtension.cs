using System.IO;
using System.Text.Json;

namespace _build.Extensions;

public static class FileInfoExtension
{
    public static string GetGrpcServiceName(this FileInfo file)
    {
        return $"Grpc{file.GetFileNameWithoutExtension().GetGrpcServiceName()}";
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

    public static string GetFileExtension(this FileInfo file)
    {
        return Path.GetExtension(file.FullName);
    }

    public static string GetFileNameWithoutExtension(this FileInfo file)
    {
        return Path.GetFileNameWithoutExtension(file.FullName);
    }
}
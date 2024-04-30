using System.IO;
using System.Text.Json;

namespace _build.Extensions;

public static class FileInfoExtension
{
    public static string GetGrpcServiceName(this FileInfo file) =>
        $"Grpc{file.GetFileNameWithoutExtension().GetGrpcServiceName()}";

    public static JsonDocument GetJsonDocument(this FileInfo file) => JsonDocument.Parse(file.ReadAllText());

    public static string ReadAllText(this FileInfo file) => File.ReadAllText(file.FullName);

    public static void WriteAllText(this FileInfo file, string text) => File.WriteAllText(file.FullName, text);

    public static string GetFileNameWithoutExtension(this FileInfo file) =>
        Path.GetFileNameWithoutExtension(file.FullName);
}
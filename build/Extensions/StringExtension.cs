using System.IO;

namespace _build.Extensions;

public static class StringExtension
{
    public static string GetGrpcServiceName(this string serviceName) =>
        $"Grpc{serviceName.Substring(6).Replace(".", "")}";

    public static FileInfo ToFile(this string path) => new(path);

    public static DirectoryInfo ToFolder(this string path) => new(path);
}
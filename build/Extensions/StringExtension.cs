using System.IO;

namespace _build.Extensions;

public static class StringExtension
{
    public static string GetGrpcServiceName(this string serviceName)
    {
        return $"Grpc{serviceName.Substring(6).Replace(".", "")}";
    }
    
    public static FileInfo ToFile(this string path)
    {
        return new FileInfo(path);
    }
    
    public static DirectoryInfo ToFolder(this string path)
    {
        return new DirectoryInfo(path);
    }
}
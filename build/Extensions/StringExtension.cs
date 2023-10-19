using System.IO;

namespace _build.Extensions;

public static class StringExtension
{
    public static FileInfo ToFile(this string path)
    {
        return new FileInfo(path);
    }
    
    public static DirectoryInfo ToFolder(this string path)
    {
        return new DirectoryInfo(path);
    }
}
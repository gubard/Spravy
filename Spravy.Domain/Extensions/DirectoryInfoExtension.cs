namespace Spravy.Domain.Extensions;

public static class DirectoryInfoExtension
{
    public static FileInfo ToFile(this DirectoryInfo directory, string fileName)
    {
        return new FileInfo(Path.Combine(directory.FullName, fileName));
    }
}
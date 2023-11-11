namespace Spravy.Domain.Extensions;

public static class DirectoryInfoExtension
{
    public static FileInfo ToFile(this DirectoryInfo directory, string fileName)
    {
        return new FileInfo(Path.Combine(directory.FullName, fileName));
    }

    public static DirectoryInfo CreateIfNotExists(this DirectoryInfo directory)
    {
        if (directory.Exists)
        {
            return directory;
        }

        directory.Create();

        return directory;
    }
}
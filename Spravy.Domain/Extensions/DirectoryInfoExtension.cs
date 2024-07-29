namespace Spravy.Domain.Extensions;

public static class DirectoryInfoExtension
{
    public static DirectoryInfo Combine(this DirectoryInfo directory, string part)
    {
        return new(Path.Combine(directory.FullName, part));
    }

    public static FileInfo ToFile(this DirectoryInfo directory, string fileName)
    {
        return new(Path.Combine(directory.FullName, fileName));
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

    public static DirectoryInfo DeleteIfExists(this DirectoryInfo directory, bool recursive)
    {
        if (!directory.Exists)
        {
            return directory;
        }

        directory.Delete(recursive);

        return directory;
    }
}

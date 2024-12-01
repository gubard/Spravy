using System.IO;
using Serilog;

namespace _build.Extensions;

public static class DirectoryInfoExtension
{
    public static FileInfo ToFile(this DirectoryInfo folder, string fileName) =>
        Path.Combine(folder.FullName, fileName).ToFile();

    public static DirectoryInfo Combine(this DirectoryInfo folder, params string[] parts)
    {
        var values = new string[parts.Length + 1];
        values[0] = folder.FullName;

        for (var i = 1; i < values.Length; i++)
        {
            values[i] = parts[i - 1];
        }

        return Path.Combine(values).ToFolder();
    }

    public static void CreateIfNotExits(this DirectoryInfo folder)
    {
        if (folder.Exists)
        {
            return;
        }

        folder.Create();
        Log.Logger.Information("Created {Folder}", folder);
    }

    public static void DeleteIfExits(this DirectoryInfo folder)
    {
        if (!folder.Exists)
        {
            return;
        }

        folder.Delete(true);
        Log.Logger.Information("Delete {Folder}", folder);
    }
}
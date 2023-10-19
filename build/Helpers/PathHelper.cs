using System.IO;

namespace _build.Helpers;

public static class PathHelper
{
    public static readonly DirectoryInfo TempFolder = new(Path.Combine("/", "tmp", "Spravy"));
    public static readonly DirectoryInfo PublishFolder = new(Path.Combine(TempFolder.FullName, "Publish"));
    public static readonly DirectoryInfo ServicesFolder = new(Path.Combine(TempFolder.FullName, "services"));
}
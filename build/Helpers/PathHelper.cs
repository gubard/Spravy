using System.IO;
using _build.Extensions;

namespace _build.Helpers;

public static class PathHelper
{
    public static readonly DirectoryInfo TempFolder = new(Path.Combine("/", "tmp", "spravy"));
    public static readonly DirectoryInfo PublishFolder = TempFolder.Combine("publish");
    public static readonly DirectoryInfo ServicesFolder = TempFolder.Combine("services");
    public static readonly DirectoryInfo BrowserFolder = "/".ToFolder().Combine("var", "www", "spravy.com.ua", "html");
    public static readonly DirectoryInfo BrowserDownloadsFolder = BrowserFolder.Combine("downloads");
}
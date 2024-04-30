using System.IO;
using _build.Interfaces;

namespace _build.Extensions;

public static class OptionsExtension
{
    public static DirectoryInfo GetAppFolder<TOptions>(this TOptions options)
        where TOptions : ICsprojFile, IFtpOptions => options.GetAppsFolder().Combine(options.GetProjectName());

    public static FileInfo GetAppDll<TOptions>(this TOptions options) where TOptions : ICsprojFile, IFtpOptions =>
        options.GetAppFolder().ToFile($"{options.GetProjectName()}.dll");
}
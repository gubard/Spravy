using System.IO;
using _build.Interfaces;
using FluentFTP;
using Serilog;

namespace _build.Extensions;

public static class FtpOptionsExtension
{
    public static FtpClient CreateFtpClient(this IFtpOptions options)
    {
        Log.Logger.Information("Connecting FTP {FtpHost} {FtpUser}", options.FtpHost, options.FtpUser);
        var values = options.FtpHost.Split(":");

        if (values.Length == 2)
        {
            return new(values[0], options.FtpUser, options.FtpPassword, int.Parse(values[1]));
        }

        return new(options.FtpHost, options.FtpUser, options.FtpPassword);
    }

    public static DirectoryInfo GetAppsFolder(this IFtpOptions options) =>
        $"/home/{options.FtpUser}/Apps".ToFolder();
}
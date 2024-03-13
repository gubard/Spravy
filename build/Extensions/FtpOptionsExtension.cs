using System.IO;
using _build.Interfaces;
using FluentFTP;
using Serilog;

namespace _build.Extensions;

public static class FtpOptionsExtension
{
    public static FtpClient CreateFtpClient(this IFtpOptions options)
    {
        var values = options.FtpHost.Split(":");
        Log.Logger.Information("Connecting {FtpHost} {FtpUser}", options.FtpHost, options.FtpUser);

        if (values.Length == 2)
        {
            return new FtpClient(values[0], options.FtpUser, options.FtpPassword, int.Parse(values[1]));
        }

        return new FtpClient(options.FtpHost, options.FtpUser, options.FtpPassword);
    }

    public static DirectoryInfo GetAppsFolder(this IFtpOptions options)
    {
        return  $"/home/{options.FtpUser}/Apps".ToFolder();
    }
}
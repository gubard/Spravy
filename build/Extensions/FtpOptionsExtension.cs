using _build.Interfaces;
using FluentFTP;

namespace _build.Extensions;

public static class FtpOptionsExtension
{
    public static FtpClient CreateFtpClient(this IFtpOptions options)
    {
        var values = options.FtpHost.Split(":");

        if (values.Length == 2)
        {
            return new FtpClient(values[0], options.FtpUser, options.FtpPassword, int.Parse(values[1]));
        }

        return new FtpClient(options.FtpHost, options.FtpUser, options.FtpPassword);
    }
}
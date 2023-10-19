using System.IO;
using FluentFTP;
using Serilog;

namespace _build.Extensions;

public static class FtpClientExtension
{
    public static void DeleteIfExistsFolder(this FtpClient client, DirectoryInfo folder)
    {
        if (!client.DirectoryExists(folder.FullName))
        {
            return;
        }

        var options = FtpListOption.Recursive | FtpListOption.ForceList | FtpListOption.Auto | FtpListOption.AllFiles;

        try
        {
            client.DeleteDirectory(folder.FullName, options);
        }
        catch
        {
            Log.Error("{Path}", folder.FullName);

            throw;
        }
    }
    
    public static void CreateIfNotExistsDirectory(this FtpClient client, DirectoryInfo folder)
    {
        if (!client.DirectoryExists(folder.FullName))
        {
            client.CreateDirectory(folder.FullName);
        }
    }
}
using System.IO;
using FluentFTP;
using Serilog;

namespace _build.Extensions;

public static class FtpClientExtension
{
    public static void CreateIfNotExistsDirectory(this FtpClient client, string path)
    {
        if (client.DirectoryExists(path))
        {
            return;
        }

        client.CreateDirectory(path, true);
    }

    public static void DeleteIfExistsDirectory(this FtpClient client, DirectoryInfo folder)
    {
        if (!client.DirectoryExists(folder.FullName))
        {
            return;
        }

        try
        {
            client.DeleteDirectory(folder.FullName,
                FtpListOption.Recursive | FtpListOption.ForceList | FtpListOption.Auto | FtpListOption.AllFiles
            );
        }
        catch
        {
            Log.Error("{Path}", folder);

            throw;
        }
    }

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
            Log.Logger.Information("Delete FTP folder {Folder}", folder);
        }
        catch
        {
            Log.Error("{Path}", folder.FullName);

            throw;
        }
    }

    public static void CreateIfNotExistsDirectory(this FtpClient client, DirectoryInfo folder)
    {
        if (client.DirectoryExists(folder.FullName))
        {
            return;
        }

        client.CreateDirectory(folder.FullName);
        Log.Logger.Information("Create FTP folder {Folder}", folder);
    }
}
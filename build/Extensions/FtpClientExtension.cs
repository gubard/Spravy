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

        var options =
            FtpListOption.Recursive
            | FtpListOption.ForceList
            | FtpListOption.Auto
            | FtpListOption.AllFiles;
        client.DeleteDirectory(folder.FullName, options);
        Log.Logger.Information("Delete FTP folder {Folder}", folder);
    }

    public static void CreateIfNotExistsFolder(this FtpClient client, DirectoryInfo folder)
    {
        if (client.DirectoryExists(folder.FullName))
        {
            return;
        }

        client.CreateDirectory(folder.FullName);
        Log.Logger.Information("Create FTP folder {Folder}", folder);
    }

    public static void UploadDirectory(
        this FtpClient client,
        DirectoryInfo localFolder,
        DirectoryInfo remoteFolder
    )
    {
        client.UploadDirectory(localFolder.FullName, remoteFolder.FullName);
        Log.Logger.Information("Upload {LocalFolder} to {RemoteFolder}", localFolder, remoteFolder);
    }
}

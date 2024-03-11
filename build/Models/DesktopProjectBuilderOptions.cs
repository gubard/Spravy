using System.IO;
using _build.Interfaces;

namespace _build.Models;

public class DesktopProjectBuilderOptions : IFtpOptions
{
    public DesktopProjectBuilderOptions(string ftpHost, string ftpUser, string ftpPassword, DirectoryInfo publishFolder)
    {
        FtpHost = ftpHost;
        FtpUser = ftpUser;
        FtpPassword = ftpPassword;
        PublishFolder = publishFolder;
    }

    public string FtpHost { get; }
    public string FtpUser { get; }
    public string FtpPassword { get; }
    public DirectoryInfo PublishFolder { get; }
}
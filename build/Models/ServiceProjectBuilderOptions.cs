using System.IO;
using _build.Interfaces;

namespace _build.Models;

public class ServiceProjectBuilderOptions : IFtpOptions, ISshOptions
{
    public ServiceProjectBuilderOptions(
        ushort port,
        string token,
        string emailPassword,
        DirectoryInfo publishFolder,
        string ftpHost,
        string ftpUser,
        string ftpPassword,
        string sshHost,
        string sshUser,
        string sshPassword
    )
    {
        Port = port;
        Token = token;
        EmailPassword = emailPassword;
        PublishFolder = publishFolder;
        FtpHost = ftpHost;
        FtpUser = ftpUser;
        FtpPassword = ftpPassword;
        SshHost = sshHost;
        SshUser = sshUser;
        SshPassword = sshPassword;
    }

    public ushort Port { get; }
    public string Token { get; }
    public string EmailPassword { get; }
    public DirectoryInfo PublishFolder { get; }
    public string FtpHost { get; }
    public string FtpUser { get; }
    public string FtpPassword { get; }
    public string SshHost { get; }
    public string SshUser { get; }
    public string SshPassword { get; }
}
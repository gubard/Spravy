using _build.Interfaces;

namespace _build.Models;

public class BrowserProjectBuilderOptions : IFtpOptions, ISshOptions
{
    public BrowserProjectBuilderOptions(
        string ftpHost,
        string ftpUser,
        string ftpPassword,
        string sshHost,
        string sshUser,
        string sshPassword
    )
    {
        FtpHost = ftpHost;
        FtpUser = ftpUser;
        FtpPassword = ftpPassword;
        SshHost = sshHost;
        SshUser = sshUser;
        SshPassword = sshPassword;
    }

    public string FtpHost { get; }
    public string FtpUser { get; }
    public string FtpPassword { get; }
    public string SshHost { get; }
    public string SshUser { get; }
    public string SshPassword { get; }
}
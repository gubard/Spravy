using System.Collections.Generic;
using System.IO;
using _build.Interfaces;

namespace _build.Models;

public class ServiceProjectBuilderOptions : ProjectBuilderOptions, IFtpOptions, ISshOptions, IPublishFolder
{
    public ServiceProjectBuilderOptions(
        FileInfo csprojFile,
        FileInfo appSettingsFile,
        IReadOnlyDictionary<string, ushort> hosts,
        IEnumerable<Runtime> runtimes,
        string configuration,
        string domain,
        ushort port,
        string token,
        string emailPassword,
        DirectoryInfo publishFolder,
        string ftpHost,
        string ftpUser,
        string ftpPassword,
        string sshHost,
        string sshUser,
        string sshPassword,
        Runtime runtime
    ) : base(
        csprojFile,
        appSettingsFile,
        hosts,
        runtimes,
        configuration,
        domain
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
        Runtime = runtime;
    }

    public ushort Port { get; }
    public string Token { get; }
    public string EmailPassword { get; }
    public Runtime Runtime { get; }
    public string FtpHost { get; }
    public string FtpUser { get; }
    public string FtpPassword { get; }
    public DirectoryInfo PublishFolder { get; }
    public string SshHost { get; }
    public string SshUser { get; }
    public string SshPassword { get; }
}
using System.Collections.Generic;
using System.IO;
using _build.Interfaces;

namespace _build.Models;

public class BrowserProjectBuilderOptions : ProjectBuilderOptions, IFtpOptions, ISshOptions
{
    public BrowserProjectBuilderOptions(
        FileInfo csprojFile,
        FileInfo appSettingsFile,
        IReadOnlyDictionary<string, ushort> hosts,
        IEnumerable<Runtime> runtimes,
        string configuration,
        string domain,
        string ftpHost,
        string ftpUser,
        string ftpPassword,
        string sshHost,
        string sshUser,
        string sshPassword,
        IEnumerable<IPublished> publisheds
    )
        : base(csprojFile, appSettingsFile, hosts, runtimes, configuration, domain)
    {
        FtpHost = ftpHost;
        FtpUser = ftpUser;
        FtpPassword = ftpPassword;
        SshHost = sshHost;
        SshUser = sshUser;
        SshPassword = sshPassword;
        Publisheds = publisheds;
    }

    public IEnumerable<IPublished> Publisheds { get; }

    public string FtpHost { get; }
    public string FtpUser { get; }
    public string FtpPassword { get; }
    public string SshHost { get; }
    public string SshUser { get; }
    public string SshPassword { get; }
}

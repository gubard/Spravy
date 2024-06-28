using System.Collections.Generic;
using System.IO;
using _build.Interfaces;

namespace _build.Models;

public class DesktopProjectBuilderOptions : ProjectBuilderOptions, IPublished
{
    public DesktopProjectBuilderOptions(
        FileInfo csprojFile,
        FileInfo appSettingsFile,
        IReadOnlyDictionary<string, ushort> hosts,
        IEnumerable<Runtime> runtimes,
        string configuration,
        string domain,
        string ftpHost,
        string ftpUser,
        string ftpPassword,
        DirectoryInfo publishFolder
    )
        : base(csprojFile, appSettingsFile, hosts, runtimes, configuration, domain)
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
    public bool IsNeedZip => true;
}

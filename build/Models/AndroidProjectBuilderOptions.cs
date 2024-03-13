using System.Collections.Generic;
using System.IO;
using _build.Interfaces;

namespace _build.Models;

public class AndroidProjectBuilderOptions : ProjectBuilderOptions, IPublished
{
    public AndroidProjectBuilderOptions(
        FileInfo csprojFile,
        FileInfo appSettingsFile,
        IReadOnlyDictionary<string, ushort> hosts,
        IEnumerable<Runtime> runtimes,
        string configuration,
        string domain,
        FileInfo keyStoreFile,
        string androidSigningKeyPass,
        string androidSigningStorePass,
        string ftpHost,
        string ftpPassword,
        string ftpUser,
        DirectoryInfo publishFolder
    ) : base(csprojFile, appSettingsFile, hosts, runtimes, configuration, domain)
    {
        KeyStoreFile = keyStoreFile;
        AndroidSigningKeyPass = androidSigningKeyPass;
        AndroidSigningStorePass = androidSigningStorePass;
        FtpHost = ftpHost;
        FtpPassword = ftpPassword;
        FtpUser = ftpUser;
        PublishFolder = publishFolder;
    }

    public FileInfo KeyStoreFile { get; }
    public string AndroidSigningKeyPass { get; }
    public string AndroidSigningStorePass { get; }
    public string FtpHost { get; }
    public string FtpUser { get; }
    public string FtpPassword { get; }
    public DirectoryInfo PublishFolder { get; }
    public bool IsNeedZip => false;
}
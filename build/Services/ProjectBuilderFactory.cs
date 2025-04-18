using System.Collections.Generic;
using System.IO;
using System.Linq;
using _build.Extensions;
using _build.Interfaces;
using _build.Models;

namespace _build.Services;

public class ProjectBuilderFactory
{
    readonly string androidSigningKeyPass;
    readonly string androidSigningStorePass;
    readonly string configuration;
    readonly string domain;
    readonly string emailAccount2Password;
    readonly string emailAccountPassword;
    readonly string emailPassword;
    readonly string ftpHost;
    readonly string ftpPassword;
    readonly string ftpUser;
    readonly FileInfo keyStoreFile;
    readonly IReadOnlyDictionary<string, ushort> ports;
    readonly DirectoryInfo publishFolder;
    readonly string sshHost;
    readonly string sshPassword;
    readonly string sshUser;
    readonly string token;
    readonly VersionService versionService;
    readonly Dictionary<Runtime, SshOptions> desktopPublishServers;
    readonly Runtime desktopRuntime;

    public ProjectBuilderFactory(
        string configuration,
        string emailPassword,
        string token,
        IReadOnlyDictionary<string, ushort> ports,
        VersionService versionService,
        DirectoryInfo publishFolder,
        string ftpHost,
        string ftpUser,
        string ftpPassword,
        string sshHost,
        string sshUser,
        string sshPassword,
        string domain,
        FileInfo keyStoreFile,
        string androidSigningKeyPass,
        string androidSigningStorePass,
        string emailAccountPassword,
        string emailAccount2Password,
        Dictionary<Runtime, SshOptions> desktopPublishServers,
        Runtime desktopRuntime
    )
    {
        this.versionService = versionService;
        this.publishFolder = publishFolder;
        this.ftpHost = ftpHost;
        this.ftpUser = ftpUser;
        this.ftpPassword = ftpPassword;
        this.sshHost = sshHost;
        this.sshUser = sshUser;
        this.sshPassword = sshPassword;
        this.domain = domain;
        this.keyStoreFile = keyStoreFile;
        this.androidSigningKeyPass = androidSigningKeyPass;
        this.androidSigningStorePass = androidSigningStorePass;
        this.emailAccountPassword = emailAccountPassword;
        this.emailAccount2Password = emailAccount2Password;
        this.desktopPublishServers = desktopPublishServers;
        this.desktopRuntime = desktopRuntime;
        this.emailPassword = emailPassword;
        this.token = token;
        this.configuration = configuration;
        this.ports = ports;
    }

    public IEnumerable<IProjectBuilder> Create(IEnumerable<FileInfo> csprojFiles)
    {
        var publishFolders = new List<DirectoryInfo>();

        foreach (var csprojFile in csprojFiles)
        {
            var projectName = csprojFile.GetFileNameWithoutExtension();

            if (projectName == "Spravy.Service")
            {
                continue;
            }

            if (projectName.EndsWith(".Tests"))
            {
                var projectBuilderOptions = new TestProjectBuilderOptions(
                    csprojFile,
                    csprojFile.Directory.ToFile("testsettings.json"),
                    ports,
                    Enumerable.Empty<Runtime>(),
                    configuration,
                    domain,
                    emailAccountPassword,
                    emailAccount2Password
                );

                yield return new TestProjectBuilder(projectBuilderOptions, versionService.Version);
            }

            if (projectName.EndsWith(".Service"))
            {
                yield return new ServiceProjectBuilder(
                    versionService.Version,
                    new(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        ports,
                        new[]
                        {
                            Runtime.LinuxX64,
                        },
                        configuration,
                        domain,
                        ports[csprojFile.GetFileNameWithoutExtension().GetGrpcServiceName()],
                        token,
                        emailPassword,
                        publishFolder.Combine(projectName),
                        ftpHost,
                        ftpUser,
                        ftpPassword,
                        sshHost,
                        sshUser,
                        sshPassword,
                        Runtime.LinuxX64
                    )
                );
            }

            if (projectName.EndsWith(".Android"))
            {
                var options = new AndroidProjectBuilderOptions(
                    csprojFile,
                    csprojFile.Directory.ToFile("appsettings.json"),
                    ports,
                    new[]
                    {
                        Runtime.AndroidArm64,
                    },
                    configuration,
                    domain,
                    keyStoreFile,
                    androidSigningKeyPass,
                    androidSigningStorePass,
                    ftpHost,
                    ftpPassword,
                    ftpUser,
                    publishFolder.Combine(projectName)
                );

                publishFolders.Add(publishFolder.Combine(projectName));

                yield return new AndroidProjectBuilder(
                    versionService.Version,
                    options
                );
            }

            if (projectName.EndsWith(".Browser"))
            {
                yield return new BrowserProjectBuilder(
                    versionService.Version,
                    new(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        ports,
                        new[]
                        {
                            Runtime.BrowserWasm,
                        },
                        configuration,
                        domain,
                        ftpHost,
                        ftpUser,
                        ftpPassword,
                        sshHost,
                        sshUser,
                        sshPassword,
                        publishFolders,
                        publishFolder.Combine(projectName)
                    )
                );
            }

            if (projectName.EndsWith(".Desktop"))
            {
                var options = new DesktopProjectBuilderOptions(
                    csprojFile,
                    csprojFile.Directory.ToFile("appsettings.json"),
                    ports,
                    new[]
                    {
                        desktopRuntime,
                    },
                    configuration,
                    domain,
                    ftpHost,
                    ftpUser,
                    ftpPassword,
                    publishFolder.Combine(projectName),
                    desktopPublishServers
                );

                publishFolders.Add(publishFolder.Combine(projectName));

                yield return new DesktopProjectBuilder(
                    versionService.Version,
                    options
                );
            }
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _build.Extensions;
using _build.Interfaces;
using _build.Models;

namespace _build.Services;

public class ProjectBuilderFactory
{
    readonly string configuration;
    readonly string token;
    readonly string emailPassword;
    readonly IReadOnlyDictionary<string, ushort> ports;
    readonly VersionService versionService;
    readonly DirectoryInfo publishFolder;
    readonly string ftpHost;
    readonly string ftpUser;
    readonly string ftpPassword;
    readonly string sshHost;
    readonly string sshUser;
    readonly string sshPassword;
    readonly string domain;
    readonly FileInfo keyStoreFile;
    readonly string androidSigningKeyPass;
    readonly string androidSigningStorePass;

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
        string androidSigningStorePass
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
        this.emailPassword = emailPassword;
        this.token = token;
        this.configuration = configuration;
        this.ports = ports;
    }

    public IEnumerable<IProjectBuilder> Create(IEnumerable<FileInfo> csprojFiles)
    {
        foreach (var csprojFile in csprojFiles)
        {
            var fileName = csprojFile.GetFileNameWithoutExtension();

            if (fileName == "Spravy.Service")
            {
                continue;
            }

            if (fileName.EndsWith(".Tests"))
            {
                yield return new TestProjectBuilder(
                    new ProjectBuilderOptions(
                        csprojFile,
                        csprojFile.Directory.ToFile("testsettings.json"),
                        ports,
                        Enumerable.Empty<Runtime>(),
                        configuration,
                        domain
                    ),
                    versionService
                );
            }

            if (fileName.EndsWith(".Service"))
            {
                yield return new ServiceProjectBuilder(
                    new ProjectBuilderOptions(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        ports,
                        new[]
                        {
                            Runtime.LinuxX64,
                        },
                        configuration,
                        domain
                    ),
                    versionService,
                    new ServiceProjectBuilderOptions(
                        ports[csprojFile.GetFileNameWithoutExtension().GetGrpcServiceName()],
                        token,
                        emailPassword,
                        publishFolder.Combine(csprojFile.GetFileNameWithoutExtension()),
                        ftpHost,
                        ftpUser,
                        ftpPassword,
                        sshHost,
                        sshUser,
                        sshPassword
                    )
                );
            }

            if (fileName.EndsWith(".Android"))
            {
                yield return new AndroidProjectBuilder(
                    new ProjectBuilderOptions(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        ports,
                        Enumerable.Empty<Runtime>(),
                        configuration,
                        domain
                    ),
                    versionService,
                    new AndroidProjectBuilderOptions(
                        keyStoreFile,
                        AndroidSigningKeyPass,
                        AndroidSigningStorePass,
                        ftpHost,
                        ftpUser,
                        ftpPassword,
                        publishFolder.Combine("Android")
                    )
                );
            }

            if (fileName.EndsWith(".Browser"))
            {
                yield return new BrowserProjectBuilder(
                    new ProjectBuilderOptions(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        ports,
                        new[]
                        {
                            Runtime.BrowserWasm,
                        },
                        configuration,
                        domain
                    ),
                    versionService,
                    new BrowserProjectBuilderOptions(
                        ftpHost,
                        ftpUser,
                        ftpPassword,
                        sshHost,
                        sshUser,
                        sshPassword
                    )
                );
            }

            if (fileName.EndsWith(".Desktop"))
            {
                yield return new DesktopProjectBuilder(
                    new ProjectBuilderOptions(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        ports,
                        new[]
                        {
                            Runtime.LinuxX64, Runtime.WinX64,
                        },
                        configuration,
                        domain
                    ),
                    versionService,
                    new DesktopProjectBuilderOptions(
                        ftpHost,
                        ftpUser,
                        ftpPassword,
                        publishFolder.Combine(csprojFile.GetFileNameWithoutExtension())
                    )
                );
            }
        }
    }
}
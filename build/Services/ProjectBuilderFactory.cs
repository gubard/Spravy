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
            var projectName = csprojFile.GetFileNameWithoutExtension();

            if (projectName == "Spravy.Service")
            {
                continue;
            }

            if (projectName.EndsWith(".Tests"))
            {
                var projectBuilderOptions = new ProjectBuilderOptions(
                    csprojFile,
                    csprojFile.Directory.ToFile("testsettings.json"),
                    ports,
                    Enumerable.Empty<Runtime>(),
                    configuration,
                    domain
                );

                yield return new TestProjectBuilder(projectBuilderOptions, versionService);
            }

            if (projectName.EndsWith(".Service"))
            {
                yield return new ServiceProjectBuilder(
                    versionService,
                    new ServiceProjectBuilderOptions(
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
                yield return new AndroidProjectBuilder(
                    versionService,
                    new AndroidProjectBuilderOptions(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        ports,
                        Enumerable.Empty<Runtime>(),
                        configuration,
                        domain,
                        keyStoreFile,
                        androidSigningKeyPass,
                        androidSigningStorePass,
                        ftpHost,
                        ftpPassword,
                        ftpUser,
                        publishFolder.Combine(projectName)
                    )
                );
            }

            if (projectName.EndsWith(".Browser"))
            {
                yield return new BrowserProjectBuilder(
                    versionService,
                    new BrowserProjectBuilderOptions(
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
                        publishFolder.Combine(projectName)
                    )
                );
            }

            if (projectName.EndsWith(".Desktop"))
            {
                yield return new DesktopProjectBuilder(
                    versionService,
                    new DesktopProjectBuilderOptions(
                        csprojFile,
                        csprojFile.Directory.ToFile("appsettings.json"),
                        ports,
                        new[]
                        {
                            Runtime.LinuxX64, Runtime.WinX64,
                        },
                        configuration,
                        domain,
                        ftpHost,
                        ftpUser,
                        ftpPassword,
                        publishFolder.Combine(projectName)
                    )
                );
            }
        }
    }
}
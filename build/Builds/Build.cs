using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _build.Extensions;
using _build.Helpers;
using _build.Interfaces;
using _build.Models;
using _build.Services;
using Nuke.Common;
using Nuke.Common.ProjectModel;

namespace _build.Builds;

class Build : NukeBuild
{
    static DirectoryInfo AndroidFolder;

    [Parameter]
    readonly string AndroidSigningKeyPass;

    [Parameter]
    readonly string AndroidSigningStorePass;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter]
    readonly string Domain;

    [Parameter]
    readonly string EmailAccount2Password;

    [Parameter]
    readonly string EmailAccountPassword;

    [Parameter]
    readonly string FtpHost;

    [Parameter]
    readonly string FtpPassword;

    [Parameter]
    readonly string FtpUser;

    [Parameter]
    readonly string JwtAudience;

    [Parameter]
    readonly string JwtIssuer;

    [Parameter]
    readonly string JwtKey;

    [Parameter]
    readonly string MailPassword;

    [Solution]
    readonly Solution Solution;

    [Parameter]
    readonly string SshHost;

    [Parameter]
    readonly string SshPassword;

    [Parameter]
    readonly string SshUser;

    [Parameter]
    readonly string TelegramToken;

    [Parameter]
    readonly string DesktopPublishServers;

    [Parameter]
    readonly string DesktopRuntime;

    IProjectBuilder[] Projects;
    string Token;

    VersionService VersionService;

    Target ProdSetupAppSettings =>
        _ => _.Executes(
            () =>
            {
                Projects = CreateProdFactory().Create(Solution.AllProjects.Select(x => new FileInfo(x.Path))).ToArray();
                BuildHelper.SetupAppSettings(Projects);
            }
        );

    Target ProdClean => _ => _.DependsOn(ProdSetupAppSettings).Executes(() => BuildHelper.Clean(Projects));
    Target ProdRestore => _ => _.DependsOn(ProdClean).Executes(() => BuildHelper.Restore(Projects));
    Target ProdCompile => _ => _.DependsOn(ProdRestore).Executes(() => BuildHelper.Compile(Projects));

    Target ProdPublishServices =>
        _ => _.DependsOn(ProdCompile)
           .Executes(() => BuildHelper.PublishServices(Projects, SshHost, SshUser, SshPassword));

    Target ProdPublishDesktop => _ =>
        _.DependsOn(ProdPublishServices).Executes(() => BuildHelper.PublishDesktop(Projects));

    Target ProdPublishAndroid => _ =>
        _.DependsOn(ProdPublishServices).Executes(() => BuildHelper.PublishAndroid(Projects));

    Target ProdPublishBrowser =>
        _ => _.DependsOn(ProdPublishAndroid, ProdPublishDesktop)
           .Executes(
                () =>
                {
                    BuildHelper.PublishBrowser(Projects);

                    BuildHelper.SendTelegramTextMessage(
                        VersionService,
                        TelegramToken,
                        "Prod",
                        FtpHost,
                        FtpUser,
                        FtpPassword,
                        Domain
                    );

                    VersionService.Save();
                }
            );

    public static int Execute() => Execute<Build>(x => x.ProdPublishBrowser);

    protected override void OnBuildInitialized()
    {
        base.OnBuildInitialized();
        PathHelper.TempFolder.DeleteIfExits();
        Token = TokenHelper.CreteToken(JwtKey, JwtIssuer, JwtAudience);
        VersionService = new($"/home/{FtpUser}/storage/version.txt".ToFile());
        VersionService.Load();
    }

    ProjectBuilderFactory CreateProdFactory() =>
        new(
            Configuration,
            MailPassword,
            Token,
            BuildHelper.Ports,
            VersionService,
            PathHelper.PublishFolder,
            FtpHost,
            FtpUser,
            FtpPassword,
            SshHost,
            SshUser,
            SshPassword,
            Domain,
            new($"/home/{FtpUser}/storage/sign-key.keystore"),
            AndroidSigningKeyPass,
            AndroidSigningStorePass,
            EmailAccountPassword,
            EmailAccount2Password,
            new(
                DesktopPublishServers.Split(';', StringSplitOptions.RemoveEmptyEntries)
                   .Select(
                        x =>
                        {
                            var values = x.Split(',', StringSplitOptions.RemoveEmptyEntries);

                            return new KeyValuePair<Runtime, SshOptions>(
                                new(values[0]),
                                new(values[1], values[2], values[3])
                            );
                        }
                    )
            ),
            new(DesktopRuntime)
        );
}
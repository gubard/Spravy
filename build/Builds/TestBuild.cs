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
using Renci.SshNet;

namespace _build.Builds;

public class TestBuild : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution]
    readonly Solution Solution;

    [Parameter]
    readonly string MailPassword;

    [Parameter]
    readonly string StagingDomain;

    [Parameter]
    readonly string StagingFtpHost;

    [Parameter]
    readonly string StagingFtpPassword;

    [Parameter]
    readonly string StagingFtpUser;

    [Parameter]
    readonly string StagingSshHost;

    [Parameter]
    readonly string StagingSshPassword;

    [Parameter]
    readonly string StagingSshUser;

    [Parameter]
    readonly string JwtAudience;

    [Parameter]
    readonly string JwtIssuer;

    [Parameter]
    readonly string JwtKey;

    [Parameter]
    readonly string AndroidSigningKeyPass;

    [Parameter]
    readonly string AndroidSigningStorePass;

    [Parameter]
    readonly string EmailAccount2Password;

    [Parameter]
    readonly string EmailAccountPassword;

    [Parameter]
    readonly string DesktopPublishServers;

    [Parameter]
    readonly string DesktopRuntime;

    [Parameter]
    readonly string FtpHost;

    [Parameter]
    readonly string FtpPassword;

    [Parameter]
    readonly string FtpUser;

    [Parameter]
    readonly string TelegramToken;

    IProjectBuilder[] Projects;
    string Token;
    VersionService VersionService;

    protected override void OnBuildInitialized()
    {
        base.OnBuildInitialized();
        PathHelper.TempFolder.DeleteIfExits();
        Token = TokenHelper.CreteToken(JwtKey, JwtIssuer, JwtAudience);
        VersionService = new($"/home/{FtpUser}/storage/version.txt".ToFile());
        VersionService.Load();
    }

    public static int Execute() => Execute<TestBuild>(x => x.StagingPublishBrowser);
    
    ProjectBuilderFactory CreateStagingFactory() =>
        new(
            Configuration,
            MailPassword,
            Token,
            BuildHelper.Ports,
            VersionService,
            PathHelper.PublishFolder,
            StagingFtpHost,
            StagingFtpUser,
            StagingFtpPassword,
            StagingSshHost,
            StagingSshUser,
            StagingSshPassword,
            StagingDomain,
            new($"/home/{StagingFtpUser}/storage/sign-key.keystore"),
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

    Target StagingPublishAndroid =>
        _ => _.DependsOn(StagingPublishServices).Executes(() => BuildHelper.PublishAndroid(Projects));

    Target StagingSetupAppSettings =>
        _ => _.Executes(
            () =>
            {
                Projects = CreateStagingFactory()
                   .Create(Solution.AllProjects.Select(x => new FileInfo(x.Path)))
                   .ToArray();

                BuildHelper.SetupAppSettings(Projects);
            }
        );

    Target StagingBuildDocker =>
        _ => _.DependsOn(StagingSetupAppSettings)
           .Executes(
                () =>
                {
                    foreach (var serviceProjectBuilder in Projects.OfType<ServiceProjectBuilder>())
                    {
                        serviceProjectBuilder.BuildDocker();
                    }
                }
            );

    Target StagingPushDockerImage =>
        _ => _.DependsOn(StagingBuildDocker)
           .Executes(
                () =>
                {
                    foreach (var serviceProjectBuilder in Projects.OfType<ServiceProjectBuilder>())
                    {
                        serviceProjectBuilder.PushDockerImage();
                    }
                }
            );

    Target StagingClean => _ => _.DependsOn(StagingPushDockerImage).Executes(() => BuildHelper.Clean(Projects));

    Target StagingRestore => _ => _.DependsOn(StagingClean).Executes(() => BuildHelper.Restore(Projects));

    Target StagingCompile => _ => _.DependsOn(StagingRestore).Executes(() => BuildHelper.Compile(Projects));

    Target StagingPublishDesktop =>
        _ => _.DependsOn(StagingPublishServices).Executes(() => BuildHelper.PublishDesktop(Projects));

    Target StagingPublishServices =>
        _ => _.DependsOn(CleanStagingDataBase, StagingCompile)
           .Executes(() => BuildHelper.PublishServices(Projects, StagingSshHost, StagingSshUser, StagingSshPassword));

    Target CleanStagingDataBase =>
        _ => _.Executes(
            () =>
            {
                using var client = new SshClient(
                    BuildHelper.CreateSshConnection(StagingSshHost, StagingSshUser, StagingSshPassword)
                );

                client.Connect();
                client.SafeRun($"echo {StagingSshPassword} | sudo -S rm -fr /home/{StagingFtpUser}/DataBases");
            }
        );

    Target Test =>
        _ => _.DependsOn(StagingPublishBrowser)
           .Executes(
                () =>
                {
                    foreach (var project in Projects.OfType<TestProjectBuilder>())
                    {
                        project.Test();
                    }
                }
            );

    Target StagingPublishBrowser =>
        _ => _.DependsOn(StagingPublishAndroid, StagingPublishDesktop)
           .Executes(
                () =>
                {
                    BuildHelper.PublishBrowser(Projects);

                    BuildHelper.SendTelegramTextMessage(
                        VersionService,
                        TelegramToken,
                        "Staging",
                        StagingFtpHost,
                        StagingFtpUser,
                        StagingFtpPassword,
                        StagingDomain
                    );
                }
            );
}
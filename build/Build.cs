using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using _build.Extensions;
using _build.Helpers;
using _build.Interfaces;
using _build.Services;
using FluentFTP;
using Microsoft.IdentityModel.Tokens;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Renci.SshNet;
using Serilog;
using Telegram.Bot;

namespace _build;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main()
    {
        return Execute<Build>(x => x.ProdPublish);
    }

    VersionService VersionService;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly string FtpPassword;
    [Parameter] readonly string SshPassword;
    [Parameter] readonly string FtpHost;
    [Parameter] readonly string FtpUser;
    [Parameter] readonly string SshHost;
    [Parameter] readonly string SshUser;
    [Parameter] readonly string ServerHost;
    [Parameter] readonly string JwtKey;
    [Parameter] readonly string JwtIssuer;
    [Parameter] readonly string JwtAudience;
    [Parameter] readonly string AndroidSigningKeyPass;
    [Parameter] readonly string AndroidSigningStorePass;
    [Parameter] readonly string TelegramToken;
    [Parameter] readonly string StagingFtpPassword;
    [Parameter] readonly string StagingSshPassword;
    [Parameter] readonly string StagingFtpHost;
    [Parameter] readonly string StagingFtpUser;
    [Parameter] readonly string StagingSshHost;
    [Parameter] readonly string StagingSshUser;
    [Parameter] readonly string StagingServerHost;
    [Parameter] readonly string MailPassword;

    static DirectoryInfo AndroidFolder;
    IReadOnlyDictionary<string, ushort> Ports;

    string Token;
    IProjectBuilder[] Projects;

    [Solution] readonly Solution Solution;

    protected override void OnBuildInitialized()
    {
        base.OnBuildInitialized();
        PathHelper.TempFolder.DeleteIfExits();
        Token = CreteToken();
        VersionService = new VersionService($"/home/{FtpUser}/storage/version.txt".ToFile());
        VersionService.Load();

        Ports = new Dictionary<string, ushort>
        {
            {
                "Spravy.Authentication.Service".GetGrpcServiceName(), 5000
            },
            {
                "Spravy.EventBus.Service".GetGrpcServiceName(), 5001
            },
            {
                "Spravy.Router.Service".GetGrpcServiceName(), 5002
            },
            {
                "Spravy.Schedule.Service".GetGrpcServiceName(), 5003
            },
            {
                "Spravy.ToDo.Service".GetGrpcServiceName(), 5004
            },
        };
    }

    ProjectBuilderFactory CreateStagingFactory()
    {
        return new ProjectBuilderFactory(
            Configuration,
            MailPassword,
            Token,
            Ports,
            VersionService,
            PathHelper.PublishFolder,
            StagingFtpHost,
            StagingFtpUser,
            StagingFtpPassword,
            StagingSshHost,
            StagingSshUser,
            StagingSshPassword,
            StagingServerHost,
            new FileInfo($"/home/{StagingFtpUser}/storage/sign-key.keystore"),
            AndroidSigningKeyPass,
            AndroidSigningStorePass
        );
    }

    ProjectBuilderFactory CreateProdFactory()
    {
        return new ProjectBuilderFactory(
            Configuration,
            MailPassword,
            Token,
            Ports,
            VersionService,
            PathHelper.PublishFolder,
            FtpHost,
            FtpUser,
            FtpPassword,
            SshHost,
            SshUser,
            SshPassword,
            ServerHost,
            new FileInfo($"/home/{FtpUser}/storage/sign-key.keystore"),
            AndroidSigningKeyPass,
            AndroidSigningStorePass
        );
    }

    void SetupAppSettings()
    {
        foreach (var project in Projects)
        {
            project.Setup();
        }
    }

    void Clean()
    {
        foreach (var project in Projects)
        {
            project.Clean();
        }
    }

    void Restore()
    {
        foreach (var project in Projects)
        {
            project.Restore();
        }
    }

    void Compile()
    {
        foreach (var project in Projects)
        {
            project.Compile();
        }
    }

    void PublishServices(string sshHost, string sshUser, string sshPassword)
    {
        using var sshClient = new SshClient(CreateSshConnection(sshHost, sshUser, sshPassword));
        sshClient.Connect();

        foreach (var project in Projects.OfType<ServiceProjectBuilder>())
        {
            project.Publish();
        }

        sshClient.SafeRun($"echo {sshPassword} | sudo -S chown -R $USER:$USER /etc/letsencrypt");
        sshClient.SafeRun($"echo {sshPassword} | sudo -S systemctl daemon-reload");

        foreach (var project in Projects.OfType<ServiceProjectBuilder>())
        {
            sshClient.SafeRun(
                $"echo {project.ServiceOptions.SshPassword} | sudo -S systemctl enable {project.ServiceOptions.GetServiceName()}"
            );
            sshClient.SafeRun(
                $"echo {project.ServiceOptions.SshPassword} | sudo -S systemctl restart {project.ServiceOptions.GetServiceName()}"
            );
        }
    }

    void PublishDesktop()
    {
        foreach (var project in Projects.OfType<DesktopProjectBuilder>())
        {
            project.Publish();
        }
    }

    void PublishAndroid()
    {
        foreach (var project in Projects.OfType<AndroidProjectBuilder>())
        {
            project.Publish();
        }
    }

    void PublishBrowser()
    {
        foreach (var project in Projects.OfType<BrowserProjectBuilder>())
        {
            project.Publish();
        }
    }

    Target StagingSetupAppSettings =>
        _ => _.Executes(() =>
            {
                Projects = CreateStagingFactory()
                    .Create(Solution.AllProjects.Select(x => new FileInfo(x.Path)))
                    .ToArray();

                SetupAppSettings();
            }
        );

    Target ProdSetupAppSettings =>
        _ => _.DependsOn(StagingPublishBrowser)
            .Executes(() =>
                {
                    Projects = CreateProdFactory()
                        .Create(Solution.AllProjects.Select(x => new FileInfo(x.Path)))
                        .ToArray();

                    SetupAppSettings();
                }
            );

    Target StagingClean => _ => _.DependsOn(StagingSetupAppSettings).Executes(Clean);
    Target ProdClean => _ => _.DependsOn(ProdSetupAppSettings).Executes(Clean);

    Target StagingRestore => _ => _.DependsOn(StagingClean).Executes(Restore);
    Target ProdRestore => _ => _.DependsOn(ProdClean).Executes(Restore);

    Target StagingCompile => _ => _.DependsOn(StagingRestore).Executes(Compile);
    Target ProdCompile => _ => _.DependsOn(ProdRestore).Executes(Compile);

    Target StagingPublishServices =>
        _ => _.DependsOn(CleanStagingDataBase, StagingCompile)
            .Executes(() => PublishServices(StagingSshHost, StagingSshUser, StagingSshPassword));

    Target CleanStagingDataBase =>
        _ => _.Executes(() =>
            {
                using var client =
                    new SshClient(CreateSshConnection(StagingSshHost, StagingSshUser, StagingSshPassword));
                client.Connect();
                client.SafeRun($"echo {StagingSshPassword} | sudo -S rm -fr /home/{StagingFtpUser}/DataBases");
            }
        );

    Target Test =>
        _ => _.DependsOn(StagingPublishServices)
            .Executes(() =>
                {
                    foreach (var project in Projects.OfType<TestProjectBuilder>())
                    {
                        project.Test();
                    }
                }
            );

    Target ProdPublishServices =>
        _ => _.DependsOn(ProdCompile)
            .Executes(() => PublishServices(SshHost, SshUser, SshPassword));

    Target StagingPublishDesktop =>
        _ => _.DependsOn(Test)
            .Executes(PublishDesktop);

    Target ProdPublishDesktop => _ => _.DependsOn(ProdPublishServices).Executes(PublishDesktop);

    Target StagingPublishAndroid =>
        _ => _.DependsOn(Test)
            .Executes(PublishAndroid);

    Target ProdPublishAndroid => _ => _.DependsOn(ProdPublishServices).Executes(PublishAndroid);

    Target StagingPublishBrowser =>
        _ => _
            .DependsOn(StagingPublishAndroid, StagingPublishDesktop)
            .Executes(() =>
                {
                    PublishBrowser();
                    using var ftpClient = CreateFtpClient(StagingFtpHost, StagingFtpUser, StagingFtpPassword);
                    ftpClient.Connect();
                    var html = PathHelper.WwwFolder.Combine(StagingServerHost).Combine("html");

                    var items = ftpClient.GetListing(html.Combine("downloads").FullName, FtpListOption.Recursive)
                        .Where(x => x.Type == FtpObjectType.File
                                    && (x.Name.EndsWith(".zip") || x.Name.EndsWith(".apk") || x.Name.EndsWith(".aab"))
                        )
                        .Select(x => x.FullName.Replace(html.FullName,
                                $"{Path.GetFileNameWithoutExtension(x.Name)}{Environment.NewLine}https://{StagingServerHost}"
                            )
                        );

                    var botClient = new TelegramBotClient(TelegramToken);

                    botClient.SendTextMessageAsync(
                            "@spravy_release",
                            $"Published Staging v{VersionService.Version}{Environment.NewLine}{string.Join(Environment.NewLine, items)}"
                        )
                        .GetAwaiter()
                        .GetResult();
                }
            );

    Target ProdPublishBrowser =>
        _ => _
            .DependsOn(ProdPublishAndroid, ProdPublishDesktop)
            .Executes(PublishBrowser);

    Target ProdPublish =>
        _ => _.DependsOn(ProdPublishBrowser)
            .Executes(() =>
                {
                    using var ftpClient = CreateFtpClient(FtpHost, FtpUser, FtpPassword);
                    ftpClient.Connect();
                    var html = PathHelper.WwwFolder.Combine(ServerHost).Combine("html");

                    var items = ftpClient.GetListing(html.Combine("downloads").FullName, FtpListOption.Recursive)
                        .Where(x => x.Type == FtpObjectType.File
                                    && (x.Name.EndsWith(".zip") || x.Name.EndsWith(".apk") || x.Name.EndsWith(".aab"))
                        )
                        .Select(x => x.FullName.Replace(html.FullName,
                                $"{Path.GetFileNameWithoutExtension(x.Name)}{Environment.NewLine}https://{ServerHost}"
                            )
                        );

                    var botClient = new TelegramBotClient(TelegramToken);

                    botClient.SendTextMessageAsync(
                            "@spravy_release",
                            $"Published Prod v{VersionService.Version}{Environment.NewLine}{string.Join(Environment.NewLine, items)}"
                        )
                        .GetAwaiter()
                        .GetResult();
                }
            );

    string CreteToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var expires = DateTime.UtcNow.AddDays(30);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "Service"),
        };

        var token = new JwtSecurityToken(
            JwtIssuer,
            JwtAudience,
            claims,
            expires: expires,
            signingCredentials: signingCredentials
        );

        var jwt = jwtSecurityTokenHandler.WriteToken(token);

        return jwt;
    }

    ConnectionInfo CreateSshConnection(string sshHost, string sshUser, string sshPassword)
    {
        var values = sshHost.Split(":");
        var password = new PasswordAuthenticationMethod(sshUser, sshPassword);

        if (values.Length == 2)
        {
            return new ConnectionInfo(values[0], int.Parse(values[1]), sshUser, password);
        }

        return new ConnectionInfo(values[0], sshUser, password);
    }

    FtpClient CreateFtpClient(string ftpHost, string ftpUser, string ftpPassword)
    {
        var values = ftpHost.Split(":");
        Log.Logger.Information("Connecting {FtpHost} {FtpUser}", ftpHost, ftpUser);

        if (values.Length == 2)
        {
            return new FtpClient(values[0], ftpUser, ftpPassword, int.Parse(values[1]));
        }

        return new FtpClient(ftpHost, ftpUser, ftpPassword);
    }
}
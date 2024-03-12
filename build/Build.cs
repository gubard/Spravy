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
using _build.Models;
using _build.Services;
using CliWrap;
using FluentFTP;
using Microsoft.IdentityModel.Tokens;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
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
        return Execute<Build>(x => x.Publish);
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

    static readonly Dictionary<string, string> Hosts = new();
    static readonly Dictionary<Project, ServiceOptions> ServiceOptions = new();
    static readonly List<Project> ServiceProjects = new();
    static DirectoryInfo AndroidFolder;
    IReadOnlyDictionary<string, ushort> Ports;

    string Token;
    IProjectBuilder[] Projects;

    [Solution] readonly Solution Solution;

    protected override void OnBuildInitialized()
    {
        base.OnBuildInitialized();
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
            new AndroidProjectBuilderOptions(
                new FileInfo($"/home/{StagingFtpUser}/storage/sign-key.keystore"),
                AndroidSigningKeyPass,
                AndroidSigningStorePass,
                StagingFtpHost,
                StagingFtpUser,
                StagingFtpPassword,
                PathHelper.PublishFolder.Combine("Android")
            ),
            VersionService,
            PathHelper.PublishFolder,
            StagingFtpHost,
            StagingFtpUser,
            StagingFtpPassword,
            StagingSshHost,
            StagingSshUser,
            StagingSshPassword
        );
    }

    ProjectBuilderFactory CreateProdFactory()
    {
        return new ProjectBuilderFactory(
            Configuration,
            MailPassword,
            Token,
            Ports,
            new AndroidProjectBuilderOptions(
                new FileInfo($"/home/{FtpUser}/storage/sign-key.keystore"),
                AndroidSigningKeyPass,
                AndroidSigningStorePass,
                FtpHost,
                FtpUser,
                FtpPassword,
                PathHelper.PublishFolder.Combine("Android")
            ),
            VersionService,
            PathHelper.PublishFolder,
            FtpHost,
            FtpUser,
            FtpPassword,
            SshHost,
            SshUser,
            SshPassword
        );
    }

    void Setup(string host)
    {
        ServiceProjects.Clear();
        ServiceProjects.AddRange(Solution.GetProjects("Service"));
        ushort port = 5000;

        foreach (var serviceProject in ServiceProjects)
        {
            ServiceOptions[serviceProject] = new ServiceOptions(port, serviceProject.Name);
            Hosts[serviceProject.GetOptionsName()] = $"https://{host}:{port}";
            port++;
        }
    }

    void SetupAppSettings(string domain)
    {
        foreach (var project in Projects)
        {
            project.Setup(domain);
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

        sshClient.SafeRun($"echo {sshPassword} | sudo -S systemctl daemon-reload");
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

    void PublishBrowser(
        string ftpHost,
        string ftpUser,
        string ftpPassword,
        string sshHost,
        string sshUser,
        string sshPassword
    )
    {
        using var sshClient = new SshClient(CreateSshConnection(ftpHost, ftpUser, ftpPassword));
        sshClient.Connect();
        using var ftpClient = CreateFtpClient(sshHost, sshUser, sshPassword);
        ftpClient.Connect();
        var browserProject = Solution.AllProjects.Single(x => x.Name == "Spravy.Ui.Browser").ThrowIfNull();
        var appBundlePath = "bin/Release/net8.0/browser-wasm/AppBundle";
        var appBundleFolder = Path.Combine(browserProject.Directory, appBundlePath).ToFolder();
        ftpClient.DeleteIfExistsFolder($"/home/{ftpUser}/{browserProject.Name}".ToFolder());
        ftpClient.UploadDirectory(appBundleFolder.FullName, $"/home/{ftpUser}/{browserProject.Name}");
        sshClient.SafeRun($"echo {sshPassword} | sudo -S rm -rf /var/www/spravy.com.ua/html/*");
        sshClient.SafeRun(
            $"echo {sshPassword} | sudo -S cp -rf /home/{ftpUser}/{browserProject.Name}/* /var/www/spravy.com.ua/html"
        );
        sshClient.SafeRun(
            $"echo {sshPassword} | sudo -S cp -rf /home/{ftpUser}/Apps/Spravy.Ui.Android/com.SerhiiMaksymovFOP.Spravy-Signed.apk /var/www/spravy.com.ua/html"
        );
        sshClient.SafeRun(
            $"echo {sshPassword} | sudo -S cp -rf /home/{ftpUser}/Apps/Spravy.Ui.Android/com.SerhiiMaksymovFOP.Spravy-Signed.aab /var/www/spravy.com.ua/html"
        );
        sshClient.SafeRun(
            $"cd /home/vafnir/Apps/Spravy.Ui.Desktop/linux-x64 && echo {sshPassword} | zip -r /var/www/spravy.com.ua/html/Spravy.Linux-x64.zip ./*"
        );
        sshClient.SafeRun(
            $"cd /home/vafnir/Apps/Spravy.Ui.Desktop/win-x64 && echo {sshPassword} | zip -r /var/www/spravy.com.ua/html/Spravy.Windows-x64.zip ./*"
        );
        sshClient.SafeRun($"echo {sshPassword} | sudo -S chown -R $USER:$USER /var/www/spravy.com.ua/html");
        sshClient.SafeRun($"echo {sshPassword} | sudo -S chmod -R 755 /var/www/spravy.com.ua");
        sshClient.SafeRun($"echo {sshPassword} | sudo -S systemctl restart nginx");
        sshClient.SafeRun($"echo {sshPassword} | sudo -S systemctl reload nginx");
    }

    Target StagingSetup => _ => _.Executes(() => Setup(StagingServerHost));
    Target ProdSetup => _ => _.DependsOn(StagingPublishBrowser).Executes(() => Setup(ServerHost));

    Target StagingSetupAppSettings =>
        _ => _.DependsOn(StagingSetup)
            .Executes(() =>
                {
                    Projects = CreateProdFactory()
                        .Create(Solution.AllProjects.Select(x => new FileInfo(x.Path)))
                        .ToArray();

                    SetupAppSettings(StagingServerHost);
                }
            );

    Target ProdSetupAppSettings =>
        _ => _.DependsOn(ProdSetup)
            .Executes(() =>
                {
                    Projects = CreateStagingFactory()
                        .Create(Solution.AllProjects.Select(x => new FileInfo(x.Path)))
                        .ToArray();

                    SetupAppSettings(ServerHost);
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
            .Executes(() => PublishBrowser(
                    StagingFtpHost,
                    StagingFtpUser,
                    StagingFtpPassword,
                    StagingSshHost,
                    StagingSshUser,
                    StagingSshPassword
                )
            );

    Target ProdPublishBrowser =>
        _ => _
            .DependsOn(ProdPublishAndroid, ProdPublishDesktop)
            .Executes(() => PublishBrowser(
                    FtpHost,
                    FtpUser,
                    FtpPassword,
                    SshHost,
                    SshUser,
                    SshPassword
                )
            );

    Target Publish =>
        _ => _.DependsOn(ProdPublishBrowser)
            .Executes(() =>
                {
                    var botClient = new TelegramBotClient(TelegramToken);
                    botClient.SendTextMessageAsync(
                            chatId: "@spravy_release",
                            text: $"Published v{VersionService.Version}"
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

    FtpClient CreateFtpClient(string ftpHost, string ftpUser, string fptPassword)
    {
        var values = ftpHost.Split(":");

        if (values.Length == 2)
        {
            return new FtpClient(values[0], ftpUser, fptPassword, int.Parse(values[1]));
        }

        return new FtpClient(ftpHost, ftpUser, fptPassword);
    }
}
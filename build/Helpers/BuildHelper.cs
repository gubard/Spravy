using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _build.Extensions;
using _build.Interfaces;
using _build.Services;
using FluentFTP;
using Renci.SshNet;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace _build.Helpers;

public static class BuildHelper
{
    static BuildHelper()
    {
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
            {
                "Spravy.PasswordGenerator.Service".GetGrpcServiceName(), 5005
            },
            {
                "Spravy.Password.Service".GetGrpcServiceName(), 5005
            },
        };
    }

    public static readonly IReadOnlyDictionary<string, ushort> Ports;

    public static void SetupAppSettings(IProjectBuilder[] projects)
    {
        foreach (var project in projects)
        {
            project.Setup();
        }
    }

    public static void Clean(IProjectBuilder[] projects)
    {
        foreach (var project in projects)
        {
            project.Clean();
        }
    }

    public static void Restore(IProjectBuilder[] projects)
    {
        foreach (var project in projects)
        {
            project.Restore();
        }
    }

    public static void Compile(IProjectBuilder[] projects)
    {
        foreach (var project in projects)
        {
            project.Compile();
        }
    }

    public static void PublishDesktop(IProjectBuilder[] projects)
    {
        foreach (var project in projects.OfType<DesktopProjectBuilder>())
        {
            project.Publish();
        }
    }

    public static void PublishAndroid(IProjectBuilder[] projects)
    {
        foreach (var project in projects.OfType<AndroidProjectBuilder>())
        {
            project.Publish();
        }
    }

    public static void PublishBrowser(IProjectBuilder[] projects)
    {
        foreach (var project in projects.OfType<BrowserProjectBuilder>())
        {
            project.Publish();
        }
    }

    public static void PublishServices(IProjectBuilder[] projects, string sshHost, string sshUser, string sshPassword)
    {
        using var sshClient = new SshClient(CreateSshConnection(sshHost, sshUser, sshPassword));
        sshClient.Connect();

        foreach (var project in projects.OfType<ServiceProjectBuilder>())
        {
            project.Publish();
        }

        sshClient.SafeRun($"echo {sshPassword} | sudo -S chown -R $USER:$USER /etc/letsencrypt");
        sshClient.SafeRun($"echo {sshPassword} | sudo -S systemctl daemon-reload");

        foreach (var project in projects.OfType<ServiceProjectBuilder>())
        {
            sshClient.SafeRun(
                $"echo {project.Options.SshPassword} | sudo -S systemctl enable {project.Options.GetServiceName()}"
            );

            sshClient.SafeRun(
                $"echo {project.Options.SshPassword} | sudo -S systemctl restart {project.Options.GetServiceName()}"
            );
        }
    }

    public static ConnectionInfo CreateSshConnection(string sshHost, string sshUser, string sshPassword)
    {
        var methods = new List<AuthenticationMethod>();
        var values = sshHost.Split(":");
        var password = new PasswordAuthenticationMethod(sshUser, sshPassword);
        methods.Add(password);

        if (values.Length == 2)
        {
            return new(values[0], int.Parse(values[1]), sshUser, methods.ToArray());
        }

        return new(values[0], sshUser, methods.ToArray());
    }

    public static void SendTelegramTextMessage(
        VersionService versionService,
        string telegramToken,
        string name,
        string ftpHost,
        string ftpUser,
        string ftpPassword,
        string domain
    )
    {
        using var ftpClient = CreateFtpClient(ftpHost, ftpUser, ftpPassword);
        ftpClient.Connect();
        var html = PathHelper.WwwFolder.Combine(domain).Combine("html");

        var items = ftpClient
           .GetListing(html.Combine("downloads", versionService.Version.ToString()).FullName, FtpListOption.Recursive)
           .Where(
                x => x.Type == FtpObjectType.File
                 && (x.Name.EndsWith(".msi")
                     || x.Name.EndsWith(".zip")
                     || (x.Name.EndsWith(".apk") || x.Name.EndsWith(".aab")) && x.Name.Contains("Spravy-Signed"))
            )
           .Select(
                x => InlineKeyboardButton.WithUrl(
                    GetButtonName(x.FullName),
                    x.FullName.Replace(html.FullName, $"https://{domain}")
                )
            )
           .ToArray();

        var botClient = new TelegramBotClient(telegramToken);

        botClient.SendTextMessageAsync(
                "@spravy_release",
                $"Published {name} v{versionService.Version}({versionService.Version.Code})"
            )
           .GetAwaiter()
           .GetResult();

        var currentItems = new List<InlineKeyboardButton>();

        for (var i = 0; i < items.Length; i++)
        {
            currentItems.Add(items[i]);

            if ((i + 1) % 3 != 0)
            {
                continue;
            }

            botClient.SendTextMessageAsync("@spravy_release", "^", replyMarkup: new InlineKeyboardMarkup(currentItems))
               .GetAwaiter()
               .GetResult();

            currentItems.Clear();
        }

        if (currentItems.Count != 0)
        {
            botClient.SendTextMessageAsync("@spravy_release", "#", replyMarkup: new InlineKeyboardMarkup(currentItems))
               .GetAwaiter()
               .GetResult();
        }
    }

    static string GetButtonName(string name) =>
        Path.GetExtension(name).ToUpperInvariant() switch
        {
            ".APK" => ".APK",
            ".AAB" => ".AAB",
            ".MSI" => new FileInfo(name).DirectoryName?.ToUpperInvariant() ?? throw new NullReferenceException(),
            ".ZIP" => Path.GetExtension(Path.GetFileNameWithoutExtension(name)).ThrowIfNull().ToUpperInvariant(),
            _ => throw new ArgumentOutOfRangeException(name),
        };

    static FtpClient CreateFtpClient(string ftpHost, string ftpUser, string ftpPassword)
    {
        var values = ftpHost.Split(":");
        Log.Logger.Information("Connecting {FtpHost} {FtpUser}", ftpHost, ftpUser);

        if (values.Length == 2)
        {
            return new(values[0], ftpUser, ftpPassword, int.Parse(values[1]));
        }

        return new(ftpHost, ftpUser, ftpPassword);
    }
}
using System;
using System.IO;
using Serilog;
using _build.Extensions;
using _build.Helpers;
using _build.Models;

namespace _build.Services;

public class BrowserProjectBuilder : UiProjectBuilder<BrowserProjectBuilderOptions>
{
    public BrowserProjectBuilder(
        VersionService versionService,
        BrowserProjectBuilderOptions browserOptions
    )
        : base(browserOptions, versionService) { }

    public void Publish()
    {
        var appBundlePath = "bin/Release/net8.0-browser/browser-wasm/AppBundle";

        if (Options.CsprojFile.Directory is null)
        {
            throw new NullReferenceException();
        }

        var appBundleFolder = Path.Combine(Options.CsprojFile.Directory.FullName, appBundlePath)
            .ToFolder();

        if (appBundleFolder.Exists)
        {
            throw new($"Not exists {appBundleFolder}");
        }

        using var sshClient = Options.CreateSshClient();
        sshClient.Connect();
        using var ftpClient = Options.CreateFtpClient();
        ftpClient.Connect();
        ftpClient.DeleteIfExistsFolder(Options.GetAppFolder());
        ftpClient.UploadDirectory(appBundleFolder.FullName, Options.GetAppFolder().FullName);
        var urlFolder = PathHelper.WwwFolder.Combine(Options.Domain);
        var browserFolder = urlFolder.Combine("html");
        var browserDownloadsFolder = browserFolder.Combine("downloads");
        sshClient.RunSudo(Options, $"rm -rf {browserFolder}/*");
        var versionFolder = browserDownloadsFolder.Combine(versionService.Version.ToString());
        var currentFolder = browserDownloadsFolder.Combine("current");
        sshClient.RunSudo(Options, $"mkdir -p {versionFolder}");
        sshClient.RunSudo(Options, $"mkdir -p {currentFolder}");
        sshClient.RunSudo(Options, $" cp -rf {Options.GetAppFolder()}/* {browserFolder}");

        foreach (var published in Options.Publisheds)
        {
            if (published.IsNeedZip)
            {
                Log.Information("Zip {ProjectName}", published.GetProjectName());
                sshClient.RunSudo(
                    Options,
                    $"mkdir -p {versionFolder.Combine(published.GetProjectName())}"
                );
                sshClient.RunSudo(
                    Options,
                    $"mkdir -p {currentFolder.Combine(published.GetProjectName())}"
                );

                if (published.Runtimes.IsEmpty)
                {
                    sshClient.SafeRun(
                        $"cd {published.GetAppFolder()} && echo {Options.SshPassword} | sudo -S  zip -r {versionFolder.Combine(published.GetProjectName()).ToFile($"{published.GetProjectName()}.zip")} ./*"
                    );
                    sshClient.SafeRun(
                        $"cd {published.GetAppFolder()} && echo {Options.SshPassword} | sudo -S  zip -r {currentFolder.Combine(published.GetProjectName()).ToFile($"{published.GetProjectName()}.zip")} ./*"
                    );
                }
                else
                {
                    foreach (var runtime in published.Runtimes.Span)
                    {
                        sshClient.SafeRun(
                            $"cd {published.GetAppFolder().Combine(runtime.Name)} && echo {Options.SshPassword} | sudo -S zip -r {versionFolder.Combine(published.GetProjectName()).ToFile($"{published.GetProjectName()}.{runtime.Name}.zip")} ./*"
                        );
                        sshClient.SafeRun(
                            $"cd {published.GetAppFolder().Combine(runtime.Name)} && echo {Options.SshPassword} | sudo -S zip -r {currentFolder.Combine(published.GetProjectName()).ToFile($"{published.GetProjectName()}.{runtime.Name}.zip")} ./*"
                        );
                    }
                }
            }
            else
            {
                Log.Information("Copy {ProjectName}", published.GetProjectName());
                sshClient.RunSudo(Options, $"cp -rf {published.GetAppFolder()} {versionFolder}");
                Log.Information("Copy {ProjectName}", published.GetProjectName());
                sshClient.RunSudo(Options, $"cp -rf {published.GetAppFolder()} {currentFolder}");
            }
        }

        sshClient.RunSudo(Options, $"chown -R $USER:$USER {browserFolder}");
        sshClient.RunSudo(Options, $"chmod -R 755 {urlFolder}");
        sshClient.RunSudo(Options, "systemctl restart nginx");
        sshClient.RunSudo(Options, "systemctl reload nginx");
    }
}

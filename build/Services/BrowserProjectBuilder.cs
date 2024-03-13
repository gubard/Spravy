using System;
using System.IO;
using _build.Extensions;
using _build.Helpers;
using _build.Models;

namespace _build.Services;

public class BrowserProjectBuilder : UiProjectBuilder
{
    readonly BrowserProjectBuilderOptions browserOptions;

    public BrowserProjectBuilder(
        VersionService versionService,
        BrowserProjectBuilderOptions browserOptions
    ) : base(browserOptions, versionService)
    {
        this.browserOptions = browserOptions;
    }

    public void Publish()
    {
        var appBundlePath = "bin/Release/net8.0/browser-wasm/AppBundle";

        if (options.CsprojFile.Directory is null)
        {
            throw new NullReferenceException();
        }

        var appBundleFolder =
            Path.Combine(options.CsprojFile.Directory.FullName, appBundlePath).ToFolder();

        using var sshClient = browserOptions.CreateSshClient();
        sshClient.Connect();
        using var ftpClient = browserOptions.CreateFtpClient();
        ftpClient.Connect();
        ftpClient.DeleteIfExistsFolder(browserOptions.GetAppFolder());

        ftpClient.UploadDirectory(appBundleFolder.FullName,
            browserOptions.GetAppFolder().FullName
        );

        var urlFolder = PathHelper.WwwFolder.Combine(browserOptions.Domain);
        var browserFolder = urlFolder.Combine("html");
        var browserDownloadsFolder = browserFolder.Combine("downloads");
        sshClient.RunSudo(browserOptions, $"rm -rf {browserFolder}/*");
        var versionFolder = browserDownloadsFolder.Combine(versionService.Version.ToString());
        sshClient.RunSudo(browserOptions, $"mkdir -p {versionFolder}");
        sshClient.RunSudo(browserOptions, $" cp -rf {browserOptions.GetAppFolder()}/* {browserFolder}");

        foreach (var published in browserOptions.Publisheds)
        {
            sshClient.RunSudo(browserOptions, $"cp -rf {published.GetAppFolder()} {versionFolder}");
        }

        sshClient.RunSudo(browserOptions, $"chown -R $USER:$USER {browserFolder}");
        sshClient.RunSudo(browserOptions, $"chmod -R 755 {urlFolder}");
        sshClient.RunSudo(browserOptions, "systemctl restart nginx");
        sshClient.RunSudo(browserOptions, "systemctl reload nginx");
    }
}
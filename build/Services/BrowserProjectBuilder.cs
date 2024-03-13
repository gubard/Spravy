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

        sshClient.SafeRun(
            $"echo {browserOptions.SshPassword} | sudo -S rm -rf {PathHelper.BrowserFolder}/*"
        );

        sshClient.SafeRun(
            $"echo {browserOptions.SshPassword} | sudo -S cp -rf {browserOptions.GetAppFolder()}/* {PathHelper.BrowserFolder}"
        );

        ftpClient.CreateIfNotExistsDirectory(PathHelper.BrowserDownloadsFolder);
        var versionFolder = PathHelper.BrowserDownloadsFolder.Combine(versionService.Version.ToString());
        ftpClient.CreateIfNotExistsDirectory(versionFolder);

        foreach (var publishFolder in browserOptions.PublishFolders)
        {
            sshClient.SafeRun(
                $"echo {browserOptions.SshPassword} | sudo -S cp -rf {publishFolder.PublishFolder} {versionFolder}"
            );
        }

        sshClient.SafeRun($"echo {browserOptions.SshPassword} | sudo -S chown -R $USER:$USER {PathHelper.BrowserFolder}"
        );

        sshClient.SafeRun($"echo {browserOptions.SshPassword} | sudo -S chmod -R 755 {PathHelper.UrlFolder}");
        sshClient.SafeRun($"echo {browserOptions.SshPassword} | sudo -S systemctl restart nginx");
        sshClient.SafeRun($"echo {browserOptions.SshPassword} | sudo -S systemctl reload nginx");
    }
}
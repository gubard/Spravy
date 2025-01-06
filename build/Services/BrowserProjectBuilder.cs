using System;
using System.IO;
using System.Linq;
using _build.Extensions;
using _build.Helpers;
using _build.Models;

namespace _build.Services;

public class BrowserProjectBuilder : UiProjectBuilder<BrowserProjectBuilderOptions>
{
    public BrowserProjectBuilder(SpravyVersion version, BrowserProjectBuilderOptions browserOptions) : base(
        browserOptions,
        version
    )
    {
    }

    public void Publish()
    {
        var appBundlePath = "bin/Release/net9.0-browser/browser-wasm/AppBundle";

        if (Options.CsprojFile.Directory is null)
        {
            throw new NullReferenceException();
        }

        var appBundleFolder = Path.Combine(Options.CsprojFile.Directory.FullName, appBundlePath).ToFolder();

        if (!appBundleFolder.Exists)
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
        var versionFolder = browserDownloadsFolder.Combine(version.ToString());
        var currentFolder = browserDownloadsFolder.Combine("current");
        sshClient.RunSudo(Options, $"mkdir -p {versionFolder}");
        sshClient.RunSudo(Options, $"mkdir -p {currentFolder}");
        sshClient.RunSudo(Options, $" cp -rf {Options.GetAppFolder()}/* {browserFolder}");

        foreach (var published in Options.Downloads)
        {
            ftpClient.UploadDirectory(published, Options.GetAppsFolder());
        }

        foreach (var published in Options.Downloads)
        {
            sshClient.RunSudo(Options, $"mkdir -p {versionFolder.Combine(published.Name)}");
            sshClient.RunSudo(Options, $"mkdir -p {currentFolder.Combine(published.Name)}");

            foreach (var directory in ftpClient.GetListing(published.FullName))
            {
                var files = ftpClient.GetListing(directory.FullName);
                var file = files.FirstOrDefault(x => x.Name.EndsWith(".msi"));

                if (file is not null)
                {
                    sshClient.RunSudo(Options, $"cp -rf {file} {versionFolder.Combine(published.Name)}");
                }
                else if (files.Any(x => x.Name.EndsWith(".aab")))
                {
                    sshClient.RunSudo(Options, $"cp -rf {directory} {versionFolder.Combine(published.Name)}");
                }
                else
                {
                    sshClient.SafeRun(
                        $"cd {directory} && echo {Options.SshPassword} | sudo -S zip -r {versionFolder.Combine(published.Name).ToFile($"{published.Name}.{directory.Name}.zip")} ./*"
                    );
                }
            }
        }

        sshClient.RunSudo(Options, $"chown -R $USER:$USER {browserFolder}");
        sshClient.RunSudo(Options, $"chmod -R 755 {urlFolder}");
        sshClient.RunSudo(Options, "systemctl restart nginx");
        sshClient.RunSudo(Options, "systemctl reload nginx");
    }
}
using System;
using System.IO;
using System.Linq;
using _build.Extensions;
using _build.Helpers;
using _build.Models;
using Serilog;

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
        var appFolder = Options.GetAppFolder();
        ftpClient.DeleteIfExistsFolder(appFolder);
        ftpClient.UploadDirectory(appBundleFolder.FullName, appFolder.FullName);
        var urlFolder = PathHelper.WwwFolder.Combine(Options.Domain);
        var browserFolder = urlFolder.Combine("html");
        var browserDownloadsFolder = browserFolder.Combine("downloads");
        sshClient.RunSudo(Options, $"rm -rf {browserFolder}/*");
        var versionFolder = browserDownloadsFolder.Combine(version.ToString());
        var currentFolder = browserDownloadsFolder.Combine("current");
        sshClient.RunSudo(Options, $"mkdir -p {versionFolder}");
        sshClient.RunSudo(Options, $"mkdir -p {currentFolder}");
        sshClient.RunSudo(Options, $" cp -rf {appFolder}/* {browserFolder}");

        foreach (var published in Options.Downloads)
        {
            var app = appFolder.Parent.Combine(published.Name);
            Log.Logger.Information("Upload {LocalFolder} {RemoteFolder}", published, app);
            ftpClient.UploadDirectory(published, app);
            sshClient.RunSudo(Options, $"mkdir -p {versionFolder.Combine(published.Name)}");
            sshClient.RunSudo(Options, $"mkdir -p {currentFolder.Combine(published.Name)}");

            foreach (var directory in ftpClient.GetListing(app.FullName))
            {
                var files = ftpClient.GetListing(directory.FullName);
                var file = files.FirstOrDefault(x => x.Name.EndsWith(".msi"));

                if (file is not null)
                {
                    Log.Logger.Information("Copy {From} {To}", file.FullName, versionFolder.Combine(published.Name));
                    sshClient.RunSudo(Options, $"cp -rf {file.FullName} {versionFolder.Combine(published.Name)}");
                }
                else if (files.Any(x => x.Name.EndsWith(".aab")))
                {
                    Log.Logger.Information("Copy {From} {To}", directory.FullName, versionFolder.Combine(published.Name));
                    sshClient.RunSudo(Options, $"cp -rf {directory.FullName} {versionFolder.Combine(published.Name)}");
                }
                else
                {
                    Log.Logger.Information(
                        "Copy {From} {To}",
                        directory.FullName,
                        versionFolder.Combine(published.Name).ToFile($"{published.Name}.{directory.Name}.zip")
                    );

                    sshClient.SafeRun(
                        $"cd {directory.FullName} && echo {Options.SshPassword} | sudo -S zip -r {versionFolder.Combine(published.Name).ToFile($"{published.Name}.{directory.Name}.zip")} ./*"
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
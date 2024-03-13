using System;
using System.IO;
using _build.Extensions;
using _build.Models;
using Nuke.Common.Tools.DotNet;

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
        if (options.Runtimes.IsEmpty)
        {
            browserOptions.PublishFolder.DeleteIfExits();

            DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(options.Configuration)
                .SetProject(options.CsprojFile.FullName)
                .SetOutput(browserOptions.PublishFolder.FullName)
                .EnableNoBuild()
                .EnableNoRestore()
            );
        }
        else
        {
            foreach (var runtime in options.Runtimes.Span)
            {
                var output = browserOptions.PublishFolder.Combine(runtime.Name);
                output.DeleteIfExits();

                DotNetTasks.DotNetPublish(setting => setting.SetConfiguration(options.Configuration)
                    .SetProject(options.CsprojFile.FullName)
                    .SetOutput(output.FullName)
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .SetRuntime(runtime.Name)
                );
            }
        }

        using var sshClient = browserOptions.CreateSshClient();
        sshClient.Connect();
        using var ftpClient = browserOptions.CreateFtpClient();
        ftpClient.Connect();
        var appBundlePath = "bin/Release/net8.0/browser-wasm/AppBundle";

        if (options.CsprojFile.Directory is null)
        {
            throw new NullReferenceException();
        }

        var appBundleFolder =
            Path.Combine(options.CsprojFile.Directory.FullName, appBundlePath).ToFolder();

        ftpClient.DeleteIfExistsFolder(
            $"/home/{browserOptions.FtpUser}/{options.GetProjectName()}"
                .ToFolder()
        );

        ftpClient.UploadDirectory(appBundleFolder.FullName,
            $"/home/{browserOptions.FtpUser}/{options.GetProjectName()}"
        );

        sshClient.SafeRun(
            $"echo {browserOptions.SshPassword} | sudo -S rm -rf /var/www/spravy.com.ua/html/*"
        );

        sshClient.SafeRun(
            $"echo {browserOptions.SshPassword} | sudo -S cp -rf /home/{browserOptions.FtpUser}/{options.GetProjectName()}/* /var/www/spravy.com.ua/html"
        );

        sshClient.SafeRun(
            $"echo {browserOptions.SshPassword} | sudo -S cp -rf /home/{browserOptions.FtpUser}/Apps/Spravy.Ui.Android/com.SerhiiMaksymovFOP.Spravy-Signed.apk /var/www/spravy.com.ua/html"
        );

        sshClient.SafeRun(
            $"echo {browserOptions.SshPassword} | sudo -S cp -rf /home/{browserOptions.FtpUser}/Apps/Spravy.Ui.Android/com.SerhiiMaksymovFOP.Spravy-Signed.aab /var/www/spravy.com.ua/html"
        );

        sshClient.SafeRun(
            $"cd /home/vafnir/Apps/Spravy.Ui.Desktop/linux-x64 && echo {browserOptions.SshPassword} | zip -r /var/www/spravy.com.ua/html/Spravy.Linux-x64.zip ./*"
        );

        sshClient.SafeRun(
            $"cd /home/vafnir/Apps/Spravy.Ui.Desktop/win-x64 && echo {browserOptions.SshPassword} | zip -r /var/www/spravy.com.ua/html/Spravy.Windows-x64.zip ./*"
        );

        sshClient.SafeRun(
            $"echo {browserOptions.SshPassword} | sudo -S chown -R $USER:$USER /var/www/spravy.com.ua/html"
        );

        sshClient.SafeRun(
            $"echo {browserOptions.SshPassword} | sudo -S chmod -R 755 /var/www/spravy.com.ua"
        );

        sshClient.SafeRun($"echo {browserOptions.SshPassword} | sudo -S systemctl restart nginx");
        sshClient.SafeRun($"echo {browserOptions.SshPassword} | sudo -S systemctl reload nginx");
    }
}
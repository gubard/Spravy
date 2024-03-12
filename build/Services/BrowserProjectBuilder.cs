using System.IO;
using _build.Extensions;
using _build.Models;

namespace _build.Services;

public class BrowserProjectBuilder : UiProjectBuilder
{
    readonly BrowserProjectBuilderOptions browserProjectBuilderOptions;

    public BrowserProjectBuilder(
        ProjectBuilderOptions projectBuilderOptions,
        VersionService versionService,
        BrowserProjectBuilderOptions browserProjectBuilderOptions
    )
        : base(projectBuilderOptions, versionService)
    {
        this.browserProjectBuilderOptions = browserProjectBuilderOptions;
    }

    public void Publish()
    {
        using var sshClient = browserProjectBuilderOptions.CreateSshClient();
        sshClient.Connect();
        using var ftpClient = browserProjectBuilderOptions.CreateFtpClient();
        ftpClient.Connect();
        var appBundlePath = "bin/Release/net8.0/browser-wasm/AppBundle";

        var appBundleFolder =
            Path.Combine(projectBuilderOptions.CsprojFile.Directory.FullName, appBundlePath).ToFolder();

        ftpClient.DeleteIfExistsFolder(
            $"/home/{browserProjectBuilderOptions.FtpUser}/{projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension()}"
                .ToFolder()
        );

        ftpClient.UploadDirectory(appBundleFolder.FullName,
            $"/home/{browserProjectBuilderOptions.FtpUser}/{projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension()}"
        );

        sshClient.SafeRun(
            $"echo {browserProjectBuilderOptions.SshPassword} | sudo -S rm -rf /var/www/spravy.com.ua/html/*"
        );

        sshClient.SafeRun(
            $"echo {browserProjectBuilderOptions.SshPassword} | sudo -S cp -rf /home/{browserProjectBuilderOptions.FtpUser}/{projectBuilderOptions.CsprojFile.GetFileNameWithoutExtension()}/* /var/www/spravy.com.ua/html"
        );

        sshClient.SafeRun(
            $"echo {browserProjectBuilderOptions.SshPassword} | sudo -S cp -rf /home/{browserProjectBuilderOptions.FtpUser}/Apps/Spravy.Ui.Android/com.SerhiiMaksymovFOP.Spravy-Signed.apk /var/www/spravy.com.ua/html"
        );

        sshClient.SafeRun(
            $"echo {browserProjectBuilderOptions.SshPassword} | sudo -S cp -rf /home/{browserProjectBuilderOptions.FtpUser}/Apps/Spravy.Ui.Android/com.SerhiiMaksymovFOP.Spravy-Signed.aab /var/www/spravy.com.ua/html"
        );

        sshClient.SafeRun(
            $"cd /home/vafnir/Apps/Spravy.Ui.Desktop/linux-x64 && echo {browserProjectBuilderOptions.SshPassword} | zip -r /var/www/spravy.com.ua/html/Spravy.Linux-x64.zip ./*"
        );

        sshClient.SafeRun(
            $"cd /home/vafnir/Apps/Spravy.Ui.Desktop/win-x64 && echo {browserProjectBuilderOptions.SshPassword} | zip -r /var/www/spravy.com.ua/html/Spravy.Windows-x64.zip ./*"
        );

        sshClient.SafeRun(
            $"echo {browserProjectBuilderOptions.SshPassword} | sudo -S chown -R $USER:$USER /var/www/spravy.com.ua/html"
        );

        sshClient.SafeRun(
            $"echo {browserProjectBuilderOptions.SshPassword} | sudo -S chmod -R 755 /var/www/spravy.com.ua"
        );

        sshClient.SafeRun($"echo {browserProjectBuilderOptions.SshPassword} | sudo -S systemctl restart nginx");
        sshClient.SafeRun($"echo {browserProjectBuilderOptions.SshPassword} | sudo -S systemctl reload nginx");
    }
}
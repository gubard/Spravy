using System;
using _build.Extensions;
using _build.Models;
using CliWrap;
using Nuke.Common.Tools.DotNet;

namespace _build.Services;

public class AndroidProjectBuilder : UiProjectBuilder
{
    readonly AndroidProjectBuilderOptions androidOptions;

    public AndroidProjectBuilder(
        VersionService versionService,
        AndroidProjectBuilderOptions androidOptions
    ) : base(androidOptions, versionService)
    {
        this.androidOptions = androidOptions;
    }

    public override void Compile()
    {
    }

    public void Publish()
    {
        if (androidOptions.KeyStoreFile.Directory is null)
        {
            throw new NullReferenceException();
        }

        androidOptions.KeyStoreFile.Directory.CreateIfNotExits();

        if (!androidOptions.KeyStoreFile.Exists)
        {
            Cli.Wrap("keytool")
                .WithArguments(new[]
                    {
                        "-genkey",
                        "-v",
                        "-keystore",
                        androidOptions.KeyStoreFile.FullName,
                        "-alias",
                        "spravy",
                        "-keyalg",
                        "RSA",
                        "-keysize",
                        "2048",
                        "-validity",
                        "10000",
                        "-dname",
                        "CN=Serhii Maksymov, OU=Serhii Maksymov FOP, O=Serhii Maksymov FOP, L=Kharkiv, S=Kharkiv State, C=Ukraine",
                        "-storepass",
                        androidOptions.AndroidSigningStorePass,
                    }
                )
                .RunCommand();
        }

        for (var i = 0; i < 3; i++)
        {
            try
            {
                if (options.Runtimes.IsEmpty)
                {
                    DotNetTasks.DotNetPublish(setting => setting.SetProject(options.CsprojFile.FullName)
                        .SetProperty("AndroidKeyStore", "true")
                        .SetProperty("AndroidSigningKeyStore", androidOptions.KeyStoreFile.FullName)
                        .SetProperty("AndroidSigningKeyAlias", "spravy")
                        .SetProperty("AndroidSigningKeyPass", androidOptions.AndroidSigningKeyPass)
                        .SetProperty("AndroidSigningStorePass", androidOptions.AndroidSigningStorePass)
                        .SetProperty("AndroidSdkDirectory", "/opt/android-sdk")
                        .SetConfiguration(options.Configuration)
                        .AddProperty("Version", versionService.Version.ToString())
                        .SetOutput(androidOptions.PublishFolder.FullName)
                        .EnableNoRestore()
                    );
                }
                else
                {
                    foreach (var runtime in options.Runtimes.Span)
                    {
                        DotNetTasks.DotNetPublish(setting =>
                            setting.SetProject(options.CsprojFile.FullName)
                                .SetProperty("AndroidKeyStore", "true")
                                .SetProperty("AndroidSigningKeyStore",
                                    androidOptions.KeyStoreFile.FullName
                                )
                                .SetProperty("AndroidSigningKeyAlias", "spravy")
                                .SetProperty("AndroidSigningKeyPass", androidOptions.AndroidSigningKeyPass
                                )
                                .SetProperty("AndroidSigningStorePass",
                                    androidOptions.AndroidSigningStorePass
                                )
                                .SetProperty("AndroidSdkDirectory", "/opt/android-sdk")
                                .SetConfiguration(options.Configuration)
                                .AddProperty("Version", versionService.Version.ToString())
                                .SetOutput(androidOptions.PublishFolder.Combine(runtime.Name).FullName)
                                .EnableNoRestore()
                                .SetRuntime(runtime.Name)
                        );
                    }
                }

                break;
            }
            catch (Exception e)
            {
                if (i == 2)
                {
                    throw;
                }

                if (e.ToString().Contains("CompileAvaloniaXamlTask"))
                {
                    continue;
                }

                throw;
            }
        }

        using var ftpClient = androidOptions.CreateFtpClient();
        ftpClient.Connect();
        ftpClient.DeleteIfExistsFolder(androidOptions.GetAppFolder());
        ftpClient.CreateIfNotExistsDirectory(androidOptions.GetAppsFolder());

        ftpClient.UploadDirectory(
            androidOptions.PublishFolder.FullName,
            androidOptions.GetAppFolder().FullName
        );
    }
}
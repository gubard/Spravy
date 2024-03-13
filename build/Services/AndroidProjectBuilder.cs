using System;
using _build.Extensions;
using _build.Models;
using CliWrap;
using Nuke.Common.Tools.DotNet;

namespace _build.Services;

public class AndroidProjectBuilder : UiProjectBuilder
{
    readonly AndroidProjectBuilderOptions androidProjectBuilderOptions;

    public AndroidProjectBuilder(
        VersionService versionService,
        AndroidProjectBuilderOptions androidProjectBuilderOptions
    )
        : base(androidProjectBuilderOptions, versionService)
    {
        this.androidProjectBuilderOptions = androidProjectBuilderOptions;
    }

    public override void Compile()
    {
    }

    public void Publish()
    {
        if (androidProjectBuilderOptions.KeyStoreFile.Directory is null)
        {
            throw new NullReferenceException();
        }

        androidProjectBuilderOptions.KeyStoreFile.Directory.CreateIfNotExits();

        if (!androidProjectBuilderOptions.KeyStoreFile.Exists)
        {
            Cli.Wrap("keytool")
                .WithArguments(new[]
                    {
                        "-genkey",
                        "-v",
                        "-keystore",
                        androidProjectBuilderOptions.KeyStoreFile.FullName,
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
                        androidProjectBuilderOptions.AndroidSigningStorePass,
                    }
                )
                .RunCommand();
        }

        androidProjectBuilderOptions.PublishFolder.DeleteIfExits();
        using var ftpClient = androidProjectBuilderOptions.CreateFtpClient();
        ftpClient.Connect();

        for (var i = 0; i < 3; i++)
        {
            try
            {
                if (projectBuilderOptions.Runtimes.IsEmpty)
                {
                    DotNetTasks.DotNetPublish(setting => setting.SetProject(projectBuilderOptions.CsprojFile.FullName)
                        .SetProperty("AndroidKeyStore", "true")
                        .SetProperty("AndroidSigningKeyStore", androidProjectBuilderOptions.KeyStoreFile.FullName)
                        .SetProperty("AndroidSigningKeyAlias", "spravy")
                        .SetProperty("AndroidSigningKeyPass", androidProjectBuilderOptions.AndroidSigningKeyPass)
                        .SetProperty("AndroidSigningStorePass", androidProjectBuilderOptions.AndroidSigningStorePass)
                        .SetProperty("AndroidSdkDirectory", "/opt/android-sdk")
                        .SetConfiguration(projectBuilderOptions.Configuration)
                        .AddProperty("Version", versionService.Version.ToString())
                        .SetOutput(androidProjectBuilderOptions.PublishFolder.FullName)
                        .EnableNoRestore()
                    );
                }
                else
                {
                    foreach (var runtime in projectBuilderOptions.Runtimes.Span)
                    {
                        DotNetTasks.DotNetPublish(setting =>
                            setting.SetProject(projectBuilderOptions.CsprojFile.FullName)
                                .SetProperty("AndroidKeyStore", "true")
                                .SetProperty("AndroidSigningKeyStore",
                                    androidProjectBuilderOptions.KeyStoreFile.FullName
                                )
                                .SetProperty("AndroidSigningKeyAlias", "spravy")
                                .SetProperty("AndroidSigningKeyPass", androidProjectBuilderOptions.AndroidSigningKeyPass
                                )
                                .SetProperty("AndroidSigningStorePass",
                                    androidProjectBuilderOptions.AndroidSigningStorePass
                                )
                                .SetProperty("AndroidSdkDirectory", "/opt/android-sdk")
                                .SetConfiguration(projectBuilderOptions.Configuration)
                                .AddProperty("Version", versionService.Version.ToString())
                                .SetOutput(androidProjectBuilderOptions.PublishFolder.Combine(runtime.Name).FullName)
                                .EnableNoRestore()
                                .SetRuntime(runtime.Name)
                        );
                    }
                }
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

        ftpClient.DeleteIfExistsDirectory(androidProjectBuilderOptions.GetAppFolder());
        ftpClient.CreateIfNotExistsDirectory(androidProjectBuilderOptions.GetAppsFolder());

        ftpClient.UploadDirectory(
            androidProjectBuilderOptions.PublishFolder.FullName,
            androidProjectBuilderOptions.GetAppFolder().FullName
        );
    }
}
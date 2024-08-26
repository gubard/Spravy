using System;
using _build.Extensions;
using _build.Models;
using Nuke.Common.Tools.DotNet;

namespace _build.Services;

public class AndroidProjectBuilder : UiProjectBuilder<AndroidProjectBuilderOptions>
{
    public AndroidProjectBuilder(
        VersionService versionService,
        AndroidProjectBuilderOptions androidOptions
    )
        : base(androidOptions, versionService) { }

    public override void Compile()
    {
        return;

        for (var i = 0; i < 3; i++)
        {
            try
            {
                if (Options.Runtimes.IsEmpty)
                {
                    DotNetTasks.DotNetBuild(setting =>
                        setting
                            .SetProjectFile(Options.CsprojFile.FullName)
                            .EnableNoRestore()
                            .SetConfiguration(Options.Configuration)
                            .SetProperty("AndroidKeyStore", "true")
                            .SetProperty("AndroidSigningKeyStore", Options.KeyStoreFile.FullName)
                            .SetProperty("AndroidSigningKeyAlias", "spravy")
                            .SetProperty("AndroidSigningKeyPass", Options.AndroidSigningKeyPass)
                            .SetProperty("AndroidSigningStorePass", Options.AndroidSigningStorePass)
                            .SetProperty("AndroidSdkDirectory", "/opt/android-sdk")
                            .SetProperty("ApplicationVersion", versionService.Version.Code)
                            .AddProperty("Version", versionService.Version.ToString())
                    );
                }
                else
                {
                    foreach (var runtime in Options.Runtimes.Span)
                    {
                        DotNetTasks.DotNetBuild(setting =>
                            setting
                                .SetProjectFile(Options.CsprojFile.FullName)
                                .EnableNoRestore()
                                .SetConfiguration(Options.Configuration)
                                .SetProperty("AndroidKeyStore", "true")
                                .SetProperty(
                                    "AndroidSigningKeyStore",
                                    Options.KeyStoreFile.FullName
                                )
                                .SetProperty("AndroidSigningKeyAlias", "spravy")
                                .SetProperty("AndroidSigningKeyPass", Options.AndroidSigningKeyPass)
                                .SetProperty(
                                    "AndroidSigningStorePass",
                                    Options.AndroidSigningStorePass
                                )
                                .SetProperty("AndroidSdkDirectory", "/opt/android-sdk")
                                .SetProperty("ApplicationVersion", versionService.Version.Code)
                                .AddProperty("Version", versionService.Version.ToString())
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
    }

    public void Publish()
    {
        for (var i = 0; i < 3; i++)
        {
            try
            {
                if (Options.Runtimes.IsEmpty)
                {
                    DotNetTasks.DotNetPublish(setting =>
                        setting
                            .SetProject(Options.CsprojFile.FullName)
                            .SetProperty("AndroidKeyStore", "true")
                            .SetProperty("AndroidSigningKeyStore", Options.KeyStoreFile.FullName)
                            .SetProperty("AndroidSigningKeyAlias", "spravy")
                            .SetProperty("AndroidSigningKeyPass", Options.AndroidSigningKeyPass)
                            .SetProperty("AndroidSigningStorePass", Options.AndroidSigningStorePass)
                            .SetProperty("AndroidSdkDirectory", "/opt/android-sdk")
                            .SetProperty("ApplicationVersion", versionService.Version.Code)
                            .SetProperty("Version", versionService.Version.ToString())
                            .SetConfiguration(Options.Configuration)
                            .SetOutput(Options.PublishFolder.FullName)
                            .EnableNoRestore()
                            .DisableNoBuild()
                    );
                }
                else
                {
                    foreach (var runtime in Options.Runtimes.Span)
                    {
                        DotNetTasks.DotNetPublish(setting =>
                            setting
                                .SetProject(Options.CsprojFile.FullName)
                                .SetProperty("AndroidKeyStore", "true")
                                .SetProperty(
                                    "AndroidSigningKeyStore",
                                    Options.KeyStoreFile.FullName
                                )
                                .SetProperty("AndroidSigningKeyAlias", "spravy")
                                .SetProperty("AndroidSigningKeyPass", Options.AndroidSigningKeyPass)
                                .SetProperty(
                                    "AndroidSigningStorePass",
                                    Options.AndroidSigningStorePass
                                )
                                .SetProperty("AndroidSdkDirectory", "/opt/android-sdk")
                                .SetProperty("ApplicationVersion", versionService.Version.Code)
                                .SetProperty("Version", versionService.Version.ToString())
                                .SetConfiguration(Options.Configuration)
                                .SetOutput(Options.PublishFolder.Combine(runtime.Name).FullName)
                                .SetRuntime(runtime.Name)
                                .EnableNoRestore()
                                .DisableNoBuild()
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

        using var ftpClient = Options.CreateFtpClient();
        ftpClient.Connect();
        ftpClient.DeleteIfExistsFolder(Options.GetAppFolder());
        ftpClient.CreateIfNotExistsFolder(Options.GetAppsFolder());
        ftpClient.UploadDirectory(Options.PublishFolder, Options.GetAppFolder());
    }
}

using System;
using System.Linq;
using _build.Extensions;
using _build.Models;
using CliWrap;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;

namespace _build.Services;

public class AndroidProjectBuilder : UiProjectBuilder
{
    readonly AndroidProjectBuilderOptions androidProjectBuilderOptions;

    public AndroidProjectBuilder(
        ProjectBuilderOptions projectBuilderOptions,
        VersionService versionService,
        AndroidProjectBuilderOptions androidProjectBuilderOptions
    )
        : base(projectBuilderOptions, versionService)
    {
        this.androidProjectBuilderOptions = androidProjectBuilderOptions;
    }

    public override void Compile()
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

        for (var i = 0; i < 3; i++)
        {
            try
            {
                DotNetTasks.DotNetBuild(setting =>
                    {
                        var result = setting.SetProjectFile(projectBuilderOptions.CsprojFile.FullName)
                            .SetProperty("AndroidKeyStore", "true")
                            .SetProperty("AndroidSigningKeyStore", androidProjectBuilderOptions.KeyStoreFile.FullName)
                            .SetProperty("AndroidSigningKeyAlias", "spravy")
                            .SetProperty("AndroidSigningKeyPass", androidProjectBuilderOptions.AndroidSigningKeyPass)
                            .SetProperty("AndroidSigningStorePass", androidProjectBuilderOptions.AndroidSigningStorePass
                            )
                            .SetProperty("AndroidSdkDirectory", "/opt/android-sdk")
                            .SetConfiguration(projectBuilderOptions.Configuration)
                            .AddProperty("Version", versionService.Version.ToString());

                        if (!projectBuilderOptions.Runtimes.IsEmpty)
                        {
                            result = result.SetRuntime(projectBuilderOptions.Runtimes.ToArray()
                                .Select(x => x.Name)
                                .JoinSemicolon()
                            );
                        }

                        return result;
                    }
                );
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
}
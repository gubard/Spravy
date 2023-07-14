using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Publish);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly string AndroidSigningKeyPass;
    [Parameter] readonly string AndroidSigningStorePass;

    [Solution] readonly Solution Solution;

    Target Clean =>
        _ => _
            .Before(Restore)
            .Executes(() => DotNetClean(setting => setting.SetProject(Solution).SetConfiguration(Configuration)));

    Target Restore =>
        _ => _
            .Executes(() => DotNetRestore(setting => setting.SetProjectFile(Solution)));

    Target Compile =>
        _ => _
            .DependsOn(Restore)
            .Executes(() => DotNetBuild(setting =>
                    setting.SetProjectFile(Solution)
                        .EnableNoRestore()
                        .SetConfiguration(Configuration)
                        .EnableDisableParallel()
                )
            );

    Target Publish =>
        _ => _
            .DependsOn(Compile)
            .Executes(async () =>
                {
                    var serviceFolder = PublishProject("Spravy.Service");
                    /*var migratorFolder = PublishProject("Spravy.Db.Sqlite.Migrator");
                    var desktopFolder = PublishProject("Spravy.Ui.Desktop");
                    var browserFolder = PublishProject("Spravy.Ui.Browser");

                    var keyStoreFile = new FileInfo(Solution.Directory / "sign-key.keystore");

                    if (keyStoreFile.Exists)
                    {
                        keyStoreFile.Delete();
                    }

                    await Cli.Wrap("keytool")
                        .WithWorkingDirectory(Solution.Directory)
                        .WithArguments(
                            $"-genkey -v -keystore sign-key.keystore -alias spravy -keyalg RSA -keysize 2048 -validity 10000 -dname \"CN=Serhii Maksymov, OU=Serhii Maksymov FOP, O=Serhii Maksymov FOP, L=Kharkiv, S=Kharkiv State, C=Ukraine\" -storepass {AndroidSigningStorePass}"
                        )
                        .ExecuteAsync();

                    var androidFolder = PublishProject("Spravy.Ui.Android", setting => setting
                        .AddProperty("AndroidKeyStore", "true")
                        .AddProperty("AndroidSigningKeyStore", keyStoreFile.FullName)
                        .AddProperty("AndroidSigningKeyAlias", "spravy")
                        .AddProperty("AndroidSigningKeyPass", AndroidSigningKeyPass)
                        .AddProperty("AndroidSigningStorePass", AndroidSigningStorePass)
                    );*/
                }
            );

    DirectoryInfo PublishProject(string name, Action<DotNetPublishSettings> configurator = null)
    {
        var publishFolder = new DirectoryInfo(Path.Combine("/", "tmp", "Spravy", "publish", name));

        if (publishFolder.Exists)
        {
            publishFolder.Delete(true);
        }

        if (!publishFolder.Exists)
        {
            publishFolder.Create();
        }

        var project = Solution.Projects.Single(x => x.Name == name);

        DotNetPublish(setting =>
            {
                configurator?.Invoke(setting);

                return setting.SetConfiguration(Configuration)
                    .SetProject(project)
                    .SetOutput(publishFolder.FullName)
                    .EnableSelfContained()
                    .EnableNoRestore()
                    .EnableDisableParallel();
            }
        );

        return publishFolder;
    }
}
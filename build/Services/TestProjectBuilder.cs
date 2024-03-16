using _build.Models;
using Nuke.Common.Tools.DotNet;
using Serilog;

namespace _build.Services;

public class TestProjectBuilder : ProjectBuilder
{
    public TestProjectBuilder(ProjectBuilderOptions options, VersionService versionService)
        : base(options, versionService)
    {
    }

    public override void Setup()
    {
        Log.Logger.Information("Set app settings {File}", options.AppSettingsFile);
    }

    public void Test()
    {
        DotNetTasks.DotNetTest(s =>
            s.SetConfiguration(options.Configuration)
                .EnableNoRestore()
                .EnableNoBuild()
                .SetProjectFile(options.CsprojFile.FullName)
        );
    }
}
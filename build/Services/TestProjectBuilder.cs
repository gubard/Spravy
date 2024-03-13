using _build.Models;
using Nuke.Common.Tools.DotNet;

namespace _build.Services;

public class TestProjectBuilder : ProjectBuilder
{
    public TestProjectBuilder(ProjectBuilderOptions options, VersionService versionService)
        : base(options, versionService)
    {
    }

    public override void Setup()
    {
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
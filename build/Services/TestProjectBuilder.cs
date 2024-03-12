using _build.Models;
using Nuke.Common.Tools.DotNet;

namespace _build.Services;

public class TestProjectBuilder : ProjectBuilder
{
    public TestProjectBuilder(ProjectBuilderOptions projectBuilderOptions, VersionService versionService)
        : base(projectBuilderOptions, versionService)
    {
    }

    public override void Setup()
    {
    }

    public void Test()
    {
        DotNetTasks.DotNetTest(s =>
            s.SetConfiguration(projectBuilderOptions.Configuration)
                .EnableNoRestore()
                .EnableNoBuild()
                .SetProjectFile(projectBuilderOptions.CsprojFile.FullName)
        );
    }
}
using _build.Models;

namespace _build.Services;

public class TestProjectBuilder : ProjectBuilder
{
    public TestProjectBuilder(ProjectBuilderOptions projectBuilderOptions, VersionService versionService) 
        : base(projectBuilderOptions, versionService)
    {
    }

    public override void Setup(string host)
    {
    }
}
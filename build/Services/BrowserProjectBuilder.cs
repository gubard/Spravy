using _build.Models;

namespace _build.Services;

public class BrowserProjectBuilder : UiProjectBuilder
{
    public BrowserProjectBuilder(ProjectBuilderOptions projectBuilderOptions, VersionService versionService)
        : base(projectBuilderOptions, versionService)
    {
    }
}
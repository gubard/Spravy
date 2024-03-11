using _build.Models;

namespace _build.Services;

public class DesktopProjectBuilder : UiProjectBuilder
{
    public DesktopProjectBuilder(ProjectBuilderOptions projectBuilderOptions, VersionService versionService)
        : base(projectBuilderOptions, versionService)
    {
    }
}
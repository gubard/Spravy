using System.IO;

namespace _build.Services;

public class DesktopProjectBuilder : ProjectBuilder
{
    public DesktopProjectBuilder(FileInfo csprojFile) : base(csprojFile)
    {
    }
}
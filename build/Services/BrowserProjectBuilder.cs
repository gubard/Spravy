using System.IO;

namespace _build.Services;

public class BrowserProjectBuilder :ProjectBuilder
{
    public BrowserProjectBuilder(FileInfo csprojFile) : base(csprojFile)
    {
    }
}
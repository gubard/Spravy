using System.IO;
using _build.Interfaces;

namespace _build.Services;

public class BrowserProjectBuilder :ProjectBuilder
{
    public BrowserProjectBuilder(FileInfo csprojFile) : base(csprojFile)
    {
    }

    public override void Setup() => throw new System.NotImplementedException();
}
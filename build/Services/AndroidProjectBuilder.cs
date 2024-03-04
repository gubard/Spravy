using System.IO;
using _build.Interfaces;

namespace _build.Services;

public class AndroidProjectBuilder : ProjectBuilder
{
    public AndroidProjectBuilder(FileInfo csprojFile) : base(csprojFile)
    {
    }

    public override void Setup() => throw new System.NotImplementedException();
}
using System.IO;

namespace _build.Services;

public class AndroidProjectBuilder : ProjectBuilder
{
    public AndroidProjectBuilder(FileInfo csprojFile) : base(csprojFile)
    {
    }
}
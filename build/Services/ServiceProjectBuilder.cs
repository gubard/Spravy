using System.IO;

namespace _build.Services;

public class ServiceProjectBuilder : ProjectBuilder
{
    readonly ushort port;

    public ServiceProjectBuilder(FileInfo csprojFile, ushort port) : base(csprojFile)
    {
        this.port = port;
    }
}
using System.IO;
using _build.Interfaces;

namespace _build.Services;

public class ServiceProjectBuilder : ProjectBuilder
{
    readonly ushort port;


    public ServiceProjectBuilder(FileInfo csprojFile, ushort port) : base(csprojFile)
    {
        this.port = port;
    }

    public override void Setup() => throw new System.NotImplementedException();
}
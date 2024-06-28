using System.IO;

namespace _build.Interfaces;

public interface ICsprojFile
{
    FileInfo CsprojFile { get; }
}

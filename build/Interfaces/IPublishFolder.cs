using System.IO;

namespace _build.Interfaces;

public interface IPublishFolder
{
    DirectoryInfo PublishFolder { get; }
}
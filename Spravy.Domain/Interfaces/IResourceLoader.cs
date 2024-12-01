namespace Spravy.Domain.Interfaces;

public interface IResourceLoader
{
    Stream GetResource(string path);
}
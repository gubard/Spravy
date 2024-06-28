namespace Spravy.Domain.Services;

public class FileResourceLoader : IResourceLoader
{
    public Stream GetResource(string path)
    {
        var file = new FileInfo(path);

        return file.OpenRead();
    }
}

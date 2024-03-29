using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Services;

public class FilesObjectStorage : IObjectStorage
{
    private readonly DirectoryInfo root;
    private readonly ISerializer serializer;

    public FilesObjectStorage(DirectoryInfo root, ISerializer serializer)
    {
        if (!root.Exists)
        {
            root.Create();
        }

        this.root = root;
        this.serializer = serializer;
    }

    public Task<bool> IsExistsAsync(string id)
    {
        var file = root.ToFile(id);

        return file.Exists.ToTaskResult();
    }

    public Task DeleteAsync(string id)
    {
        var file = root.ToFile(id);
        file.Delete();

        return Task.CompletedTask;
    }

    public async Task SaveObjectAsync(string id, object obj)
    {
        var file = root.ToFile(id);

        if (file.Exists)
        {
            file.Delete();
        }

        await using var stream = file.Create();
        serializer.Serialize(obj, stream);
    }

    public async Task<TObject> GetObjectAsync<TObject>(string id)
    {
        var file = root.ToFile(id);
        await using var stream = file.OpenRead();

        return serializer.Deserialize<TObject>(stream);
    }
}
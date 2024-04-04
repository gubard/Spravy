using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

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

    public ValueTask<Result<bool>> IsExistsAsync(string id)
    {
        var file = root.ToFile(id);

        return file.Exists.ToResult().ToValueTaskResult();
    }

    public ValueTask<Result> DeleteAsync(string id)
    {
        var file = root.ToFile(id);
        file.Delete();

        return Result.SuccessValueTask;
    }

    public async ValueTask<Result> SaveObjectAsync(string id, object obj)
    {
        var file = root.ToFile(id);

        if (file.Exists)
        {
            file.Delete();
        }

        await using var stream = file.Create();

        return await serializer.Serialize(obj, stream);
    }

    public async ValueTask<Result<TObject>> GetObjectAsync<TObject>(string id)
    {
        var file = root.ToFile(id);
        await using var stream = file.OpenRead();

        return await serializer.Deserialize<TObject>(stream);
    }
}
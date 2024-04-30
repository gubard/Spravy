using System.Runtime.CompilerServices;
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

    public ConfiguredValueTaskAwaitable<Result<bool>> IsExistsAsync(string id)
    {
        var file = root.ToFile(id);

        return file.Exists.ToResult().ToValueTaskResult().ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteAsync(string id)
    {
        var file = root.ToFile(id);
        file.Delete();

        return Result.AwaitableFalse;
    }

    public ConfiguredValueTaskAwaitable<Result> SaveObjectAsync(string id, object obj)
    {
        return SaveObjectCore(id, obj).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> GetObjectAsync<TObject>(string id)
    {
        return GetObjectCore<TObject>(id).ConfigureAwait(false);
    }

    private async ValueTask<Result> SaveObjectCore(string id, object obj)
    {
        var file = root.ToFile(id);

        if (file.Exists)
        {
            file.Delete();
        }

        await using var stream = file.Create();

        return await serializer.SerializeAsync(obj, stream);
    }

    private async ValueTask<Result<TObject>> GetObjectCore<TObject>(string id)
    {
        var file = root.ToFile(id);
        await using var stream = file.OpenRead();

        return await serializer.DeserializeAsync<TObject>(stream);
    }
}
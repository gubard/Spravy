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

    public ConfiguredValueTaskAwaitable<Result<bool>> IsExistsAsync(string id, CancellationToken ct)
    {
        var file = root.ToFile(id);

        return file.Exists.ToResult().ToValueTaskResult().ConfigureAwait(false);
    }

    public Cvtar DeleteAsync(string id, CancellationToken ct)
    {
        var file = root.ToFile(id);
        file.Delete();

        return Result.AwaitableSuccess;
    }

    public Cvtar SaveObjectAsync(string id, object obj, CancellationToken ct)
    {
        return SaveObjectCore(id, obj, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> GetObjectAsync<TObject>(string id, CancellationToken ct)
        where TObject : notnull
    {
        return GetObjectCore<TObject>(id, ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> SaveObjectCore(string id, object obj, CancellationToken ct)
    {
        var file = root.ToFile(id);

        if (file.Exists)
        {
            file.Delete();
        }

        await using var stream = file.Create();

        return await serializer.SerializeAsync(obj, stream, ct);
    }

    private async ValueTask<Result<TObject>> GetObjectCore<TObject>(string id, CancellationToken ct)
        where TObject : notnull
    {
        var file = root.ToFile(id);
        await using var stream = file.OpenRead();

        return await serializer.DeserializeAsync<TObject>(stream, ct);
    }
}
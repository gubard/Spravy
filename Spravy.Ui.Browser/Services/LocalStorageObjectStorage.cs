namespace Spravy.Ui.Browser.Services;

public class LocalStorageObjectStorage : IObjectStorage
{
    private readonly ISerializer serializer;

    public LocalStorageObjectStorage(ISerializer serializer)
    {
        this.serializer = serializer;
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> IsExistsAsync(string id, CancellationToken ct)
    {
        var value = JsLocalStorageInterop.LocalStorageGetItem(id);

        return (!value.IsNullOrWhiteSpace()).ToResult().ToValueTaskResult().ConfigureAwait(false);
    }

    public Cvtar DeleteAsync(string id, CancellationToken ct)
    {
        JsLocalStorageInterop.LocalStorageRemoveItem(id);

        return Result.AwaitableSuccess;
    }

    public Cvtar SaveObjectAsync(string id, object obj, CancellationToken ct)
    {
        return SaveObjectCore(id, obj, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> GetObjectAsync<TObject>(
        string id,
        CancellationToken ct
    )
        where TObject : notnull
    {
        return GetObjectCore<TObject>(id, ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> SaveObjectCore(string id, object obj, CancellationToken ct)
    {
        await using var stream = new MemoryStream();
        var result = await serializer.SerializeAsync(obj, stream, ct);

        if (result.IsHasError)
        {
            return result;
        }

        var bytes = stream.ToArray();
        var value = Convert.ToBase64String(bytes);
        JsLocalStorageInterop.LocalStorageSetItem(id, value);

        return Result.Success;
    }

    public async ValueTask<Result<TObject>> GetObjectCore<TObject>(string id, CancellationToken ct)
        where TObject : notnull
    {
        var value = JsLocalStorageInterop.LocalStorageGetItem(id);
        var bytes = Convert.FromBase64String(value);
        await using var stream = new MemoryStream(bytes);

        return await serializer.DeserializeAsync<TObject>(stream, ct);
    }
}

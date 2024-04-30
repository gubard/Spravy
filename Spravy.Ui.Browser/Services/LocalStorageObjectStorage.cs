using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Ui.Browser.Helpers;

namespace Spravy.Ui.Browser.Services;

public class LocalStorageObjectStorage : IObjectStorage
{
    private readonly ISerializer serializer;

    public LocalStorageObjectStorage(ISerializer serializer)
    {
        this.serializer = serializer;
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> IsExistsAsync(string id)
    {
        var value = JSInterop.LocalStorageGetItem(id);

        return (!value.IsNullOrWhiteSpace()).ToResult().ToValueTaskResult().ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteAsync(string id)
    {
        JSInterop.LocalStorageRemoveItem(id);

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
        await using var stream = new MemoryStream();
        var result = await serializer.SerializeAsync(obj, stream);

        if (result.IsHasError)
        {
            return result;
        }

        var bytes = stream.ToArray();
        var value = Convert.ToBase64String(bytes);
        JSInterop.LocalStorageSetItem(id, value);

        return Result.Success;
    }

    public async ValueTask<Result<TObject>> GetObjectCore<TObject>(string id)
    {
        var value = JSInterop.LocalStorageGetItem(id);
        var bytes = Convert.FromBase64String(value);
        await using var stream = new MemoryStream(bytes);

        return await serializer.DeserializeAsync<TObject>(stream);
    }
}
using System;
using System.IO;
using System.Threading.Tasks;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Browser.Helpers;

namespace Spravy.Ui.Browser.Services;

public class LocalStorageObjectStorage : IObjectStorage
{
    private readonly ISerializer serializer;

    public LocalStorageObjectStorage(ISerializer serializer)
    {
        this.serializer = serializer;
    }

    public Task<bool> IsExistsAsync(string id)
    {
        var value = JSInterop.LocalStorageGetItem(id);

        return Task.FromResult(!value.IsNullOrWhiteSpace());
    }

    public Task DeleteAsync(string id)
    {
        JSInterop.LocalStorageRemoveItem(id);

        return Task.CompletedTask;
    }

    public async Task SaveObjectAsync(string id, object obj)
    {
        await using var stream = new MemoryStream();
        await serializer.SerializeAsync(obj, stream);
        var bytes = stream.ToArray();
        var value = Convert.ToBase64String(bytes);
        JSInterop.LocalStorageSetItem(id, value);
    }

    public async Task<TObject> GetObjectAsync<TObject>(string id)
    {
        var value = JSInterop.LocalStorageGetItem(id);
        var bytes = Convert.FromBase64String(value);
        await using var stream = new MemoryStream(bytes);

        return await serializer.DeserializeAsync<TObject>(stream);
    }
}
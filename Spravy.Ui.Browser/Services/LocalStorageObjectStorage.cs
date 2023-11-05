using System.IO;
using System.Threading.Tasks;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Browser.Helpers;

namespace Spravy.Ui.Browser.Services;

public class LocalStorageObjectStorage : IObjectStorage
{
    private readonly ISerializer serializer;
    private readonly IBytesToString bytesToString;
    private readonly IStringToBytes stringToBytes;

    public LocalStorageObjectStorage(ISerializer serializer, IBytesToString bytesToString, IStringToBytes stringToBytes)
    {
        this.serializer = serializer;
        this.bytesToString = bytesToString;
        this.stringToBytes = stringToBytes;
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
        var value = bytesToString.BytesToString(bytes);
        JSInterop.LocalStorageSetItem(id, value);
    }

    public async Task<TObject> GetObjectAsync<TObject>(string id)
    {
        var value = JSInterop.LocalStorageGetItem(id);
        var bytes = stringToBytes.StringToBytes(value);
        await using var stream = new MemoryStream(bytes);

        return await serializer.DeserializeAsync<TObject>(stream);
    }
}
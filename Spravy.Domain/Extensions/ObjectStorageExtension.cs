using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Extensions;

public static class ObjectStorageExtension
{
    public static async Task<TObject> GetObjectOrDefaultAsync<TObject>(this IObjectStorage objectStorage, string id)
        where TObject : new()
    {
        if (await objectStorage.IsExistsAsync(id))
        {
            return await objectStorage.GetObjectAsync<TObject>(id);
        }

        return new TObject();
    }
}
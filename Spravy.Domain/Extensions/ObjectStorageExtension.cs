using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class ObjectStorageExtension
{
    public static ValueTask<Result<TObject>> GetObjectOrDefaultAsync<TObject>(
        this IObjectStorage objectStorage,
        string id
    )
        where TObject : new()
    {
        return objectStorage.IsExistsAsync(id)
            .ConfigureAwait(false)
            .IfSuccessAsync(
                value =>
                {
                    if (value)
                    {
                        return objectStorage.GetObjectAsync<TObject>(id).ConfigureAwait(false);
                    }

                    return new TObject().ToResult().ToValueTaskResult().ConfigureAwait(false);
                }
            );
    }
}
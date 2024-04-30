using System.Runtime.CompilerServices;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class ObjectStorageExtension
{
    public static ConfiguredValueTaskAwaitable<Result<TObject>> GetObjectOrDefaultAsync<TObject>(
        this IObjectStorage objectStorage,
        string id,
        CancellationToken cancellationToken
    ) where TObject : new()
    {
        return objectStorage.IsExistsAsync(id)
           .IfSuccessAsync(value =>
            {
                if (value)
                {
                    return objectStorage.GetObjectAsync<TObject>(id);
                }

                return new TObject().ToResult().ToValueTaskResult().ConfigureAwait(false);
            }, cancellationToken);
    }
}
using System.Runtime.CompilerServices;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class EmptyHttpHeaderFactory : IHttpHeaderFactory
{
    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken cancellationToken
    )
    {
        return ReadOnlyMemory<HttpHeaderItem>.Empty.ToResult().ToValueTaskResult().ConfigureAwait(false);
    }
}
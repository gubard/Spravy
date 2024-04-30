using System.Runtime.CompilerServices;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class ValuesHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly ReadOnlyMemory<HttpHeaderItem> items;

    public ValuesHttpHeaderFactory(ReadOnlyMemory<HttpHeaderItem> items)
    {
        this.items = items;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken cancellationToken
    )
    {
        return items.ToResult().ToValueTaskResult().ConfigureAwait(false);
    }
}
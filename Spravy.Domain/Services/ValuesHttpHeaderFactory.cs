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

    public Task<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(CancellationToken cancellationToken)
    {
        return items.ToResult().ToTaskResult();
    }
}
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class ValuesHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly IEnumerable<HttpHeaderItem> items;

    public ValuesHttpHeaderFactory(IEnumerable<HttpHeaderItem> items)
    {
        this.items = items;
    }

    public Task<IEnumerable<HttpHeaderItem>> CreateHeaderItemsAsync()
    {
        return Task.FromResult(items);
    }
}
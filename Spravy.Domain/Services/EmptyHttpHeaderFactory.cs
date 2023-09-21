using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class EmptyHttpHeaderFactory : IHttpHeaderFactory
{
    public Task<IEnumerable<HttpHeaderItem>> CreateHeaderItemsAsync()
    {
        return Task.FromResult(Enumerable.Empty<HttpHeaderItem>());
    }
}
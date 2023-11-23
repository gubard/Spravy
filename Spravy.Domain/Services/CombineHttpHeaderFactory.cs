using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class CombineHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly IEnumerable<IHttpHeaderFactory> factories;

    public CombineHttpHeaderFactory(params IHttpHeaderFactory[] factories)
    {
        this.factories = factories;
    }
    
    public CombineHttpHeaderFactory(IEnumerable<IHttpHeaderFactory> factories)
    {
        this.factories = factories;
    }

    public async Task<IEnumerable<HttpHeaderItem>> CreateHeaderItemsAsync(CancellationToken cancellationToken)
    {
        var result = new List<HttpHeaderItem>();

        foreach (var factory in factories)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var headers = await factory.CreateHeaderItemsAsync(cancellationToken);
            result.AddRange(headers);
        }

        return result;
    }
}
namespace Spravy.Domain.Services;

public class CombineHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly ReadOnlyMemory<IHttpHeaderFactory> factories;

    public CombineHttpHeaderFactory(params IHttpHeaderFactory[] factories)
    {
        this.factories = factories;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken ct
    )
    {
        return CreateHeaderItemsCore(ct).ConfigureAwait(false);
    }

    public async ValueTask<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsCore(
        CancellationToken ct
    )
    {
        var result = new List<HttpHeaderItem>();

        foreach (var factory in factories.ToArray())
        {
            var headers = await factory.CreateHeaderItemsAsync(ct);

            if (headers.IsHasError)
            {
                return headers;
            }

            result.AddRange(headers.Value.Span);
        }

        return result.ToReadOnlyMemory().ToResult();
    }
}
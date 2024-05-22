namespace Spravy.Domain.Services;

public class CombineHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly ReadOnlyMemory<IHttpHeaderFactory> factories;

    public CombineHttpHeaderFactory(params IHttpHeaderFactory[] factories)
    {
        this.factories = factories;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken cancellationToken
    )
    {
        return CreateHeaderItemsCore(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsCore(
        CancellationToken cancellationToken
    )
    {
        var result = new List<HttpHeaderItem>();

        foreach (var factory in factories.ToArray())
        {
            var headers = await factory.CreateHeaderItemsAsync(cancellationToken);

            if (headers.IsHasError)
            {
                return headers;
            }

            result.AddRange(headers.Value.Span);
        }

        return result.ToReadOnlyMemory().ToResult();
    }
}
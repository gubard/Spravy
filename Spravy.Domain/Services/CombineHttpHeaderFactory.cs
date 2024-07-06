namespace Spravy.Domain.Services;

public class CombineHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly ReadOnlyMemory<IHttpHeaderFactory> factories;

    public CombineHttpHeaderFactory(params IHttpHeaderFactory[] factories)
    {
        this.factories = factories;
    }

    public ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<HttpHeaderItem>>
    > CreateHeaderItemsAsync(CancellationToken ct)
    {
        return CreateHeaderItemsCore(ct).ConfigureAwait(false);
    }

    public async ValueTask<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsCore(
        CancellationToken ct
    )
    {
        var result = new List<HttpHeaderItem>();

        for (var index = 0; index < factories.Length; index++)
        {
            var headers = await factories.Span[index].CreateHeaderItemsAsync(ct);

            if (!headers.TryGetValue(out var value))
            {
                return headers;
            }

            result.AddRange(value.Span);
        }

        return result.ToReadOnlyMemory().ToResult();
    }
}

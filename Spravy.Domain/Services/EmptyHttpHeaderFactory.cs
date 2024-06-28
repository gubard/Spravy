namespace Spravy.Domain.Services;

public class EmptyHttpHeaderFactory : IHttpHeaderFactory
{
    public ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<HttpHeaderItem>>
    > CreateHeaderItemsAsync(CancellationToken ct)
    {
        return ReadOnlyMemory<HttpHeaderItem>
            .Empty.ToResult()
            .ToValueTaskResult()
            .ConfigureAwait(false);
    }
}

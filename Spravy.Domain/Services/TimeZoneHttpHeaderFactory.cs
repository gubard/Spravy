namespace Spravy.Domain.Services;

public class TimeZoneHttpHeaderFactory : IHttpHeaderFactory
{
    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken ct
    )
    {
        return HttpHeaderItem.TimeZoneOffset().ToReadOnlyMemory().ToResult().ToValueTaskResult().ConfigureAwait(false);
    }
}
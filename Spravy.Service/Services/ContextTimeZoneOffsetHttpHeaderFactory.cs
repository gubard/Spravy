namespace Spravy.Service.Services;

public class ContextTimeZoneOffsetHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public ContextTimeZoneOffsetHttpHeaderFactory(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken cancellationToken
    )
    {
        var authorization = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffsetHeader();

        return new HttpHeaderItem(HttpNames.HeaderTimeZoneOffsetName, authorization).ToReadOnlyMemory()
           .ToResult()
           .ToValueTaskResult()
           .ConfigureAwait(false);
    }
}
namespace Spravy.Service.Services;

public class ContextAccessorUserIdHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public ContextAccessorUserIdHttpHeaderFactory(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken cancellationToken
    )
    {
        var userId = httpContextAccessor.HttpContext.ThrowIfNull().GetUserId();

        return new HttpHeaderItem(HttpNames.HeaderUserIdName, userId).ToReadOnlyMemory()
           .ToResult()
           .ToValueTaskResult()
           .ConfigureAwait(false);
    }
}
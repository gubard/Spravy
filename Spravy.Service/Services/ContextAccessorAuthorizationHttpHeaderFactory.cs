namespace Spravy.Service.Services;

public class ContextAccessorAuthorizationHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public ContextAccessorAuthorizationHttpHeaderFactory(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken cancellationToken
    )
    {
        var authorization = httpContextAccessor.HttpContext.ThrowIfNull().GetAuthorizationHeader();

        return new HttpHeaderItem(HttpNames.HeaderAuthorizationName, authorization).ToReadOnlyMemory()
           .ToResult()
           .ToValueTaskResult()
           .ConfigureAwait(false);
    }
}
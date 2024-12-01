namespace Spravy.Domain.Services;

public class TokenHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly ITokenService tokenService;

    public TokenHttpHeaderFactory(ITokenService tokenService)
    {
        this.tokenService = tokenService;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken ct
    )
    {
        return tokenService.GetTokenAsync(ct)
           .IfSuccessAsync(
                value => HttpHeaderItem.CreateBearerAuthorization(value)
                   .ToReadOnlyMemory()
                   .ToResult()
                   .ToValueTaskResult()
                   .ConfigureAwait(false),
                ct
            );
    }
}
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class TokenHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly ITokenService tokenService;

    public TokenHttpHeaderFactory(ITokenService tokenService)
    {
        this.tokenService = tokenService;
    }

    public ValueTask<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken cancellationToken
    )
    {
        return tokenService.GetTokenAsync(cancellationToken)
            .ConfigureAwait(false)
            .IfSuccessAsync(
                value => HttpHeaderItem.CreateBearerAuthorization(value)
                    .ToReadOnlyMemory()
                    .ToResult()
                    .ToValueTaskResult()
                    .ConfigureAwait(false)
            );
    }
}
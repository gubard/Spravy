using System.Runtime.CompilerServices;
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

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken cancellationToken
    )
    {
        return tokenService.GetTokenAsync(cancellationToken)
            .IfSuccessAsync(
                value => HttpHeaderItem.CreateBearerAuthorization(value)
                    .ToReadOnlyMemory()
                    .ToResult()
                    .ToValueTaskResult()
                    .ConfigureAwait(false)
            );
    }
}
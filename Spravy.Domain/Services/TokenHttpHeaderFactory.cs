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

    public async Task<IEnumerable<HttpHeaderItem>> CreateHeaderItemsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var token = await tokenService.GetTokenAsync(cancellationToken);

        return HttpHeaderItem.CreateBearerAuthorization(token).ToEnumerable();
    }
}
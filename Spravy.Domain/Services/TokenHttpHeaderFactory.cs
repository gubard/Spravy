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

    public async Task<IEnumerable<HttpHeaderItem>> CreateHeaderItemsAsync()
    {
        var token = await tokenService.GetTokenAsync();

        return HttpHeaderItem.CreateBearerAuthorization(token).ToEnumerable();
    }
}
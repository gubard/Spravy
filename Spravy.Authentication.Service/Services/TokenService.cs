using System.IdentityModel.Tokens.Jwt;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Service.Services;

public class TokenService : ITokenService
{
    private readonly ITokenFactory tokenFactory;
    private TokenResult token;

    public TokenService(ITokenFactory tokenFactory)
    {
        this.tokenFactory = tokenFactory;
        token = tokenFactory.Create(
            new UserTokenClaims("authentication.service", Guid.Empty, Role.Service, string.Empty)
        );
    }

    public ValueTask<Result<string>> GetTokenAsync(CancellationToken cancellationToken)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(token.Token);
        DateTimeOffset expires = jwtToken.ValidTo;

        if (expires > DateTimeOffset.Now)
        {
            return new Result<string>(token.Token).ToValueTaskResult();
        }

        token = tokenFactory.Create(
            new UserTokenClaims("authentication.service", Guid.Empty, Role.Service, string.Empty)
        );

        return new Result<string>(token.Token).ToValueTaskResult();
    }

    public ValueTask<Result> LoginAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public ValueTask<Result> LoginAsync(string refreshToken, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}
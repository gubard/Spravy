using System.IdentityModel.Tokens.Jwt;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Enums;
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
        token = tokenFactory.Create(new UserTokenClaims("authentication.service", Guid.Empty, Role.Service));
    }

    public Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(token.Token);
        DateTimeOffset expires = jwtToken.ValidTo;

        if (expires > DateTimeOffset.Now)
        {
            return Task.FromResult(token.Token);
        }

        token = tokenFactory.Create(new UserTokenClaims("authentication.service", Guid.Empty, Role.Service));

        return Task.FromResult(token.Token);
    }

    public Task LoginAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task LoginAsync(string refreshToken, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public void Login(TokenResult tokenResult, CancellationToken cancellationToken)
    {
        token = tokenResult;
    }
}
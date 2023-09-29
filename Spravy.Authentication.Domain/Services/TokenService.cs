using System.IdentityModel.Tokens.Jwt;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Domain.Services;

public class TokenService : ITokenService
{
    private readonly IAuthenticationService authenticationService;
    private TokenResult token;

    public TokenService(IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
    }

    public async Task<string> GetTokenAsync()
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(token.Token);

        if (jwtToken.ValidTo == default)
        {
            return token.Token;
        }
        
        DateTimeOffset expires = jwtToken.ValidTo;

        if (expires > DateTimeOffset.Now)
        {
            return token.Token;
        }

        token = await authenticationService.RefreshTokenAsync(token.RefreshToken);

        return token.Token;
    }

    public async Task LoginAsync(User user)
    {
        token = await authenticationService.LoginAsync(user);
    }

    public async Task LoginAsync(string refreshToken)
    {
        token = await authenticationService.RefreshTokenAsync(refreshToken);
    }
}
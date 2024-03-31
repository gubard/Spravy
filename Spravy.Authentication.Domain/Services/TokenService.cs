using System.IdentityModel.Tokens.Jwt;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Domain.Extensions;

namespace Spravy.Authentication.Domain.Services;

public class TokenService : ITokenService
{
    private readonly IAuthenticationService authenticationService;
    private TokenResult token;

    public TokenService(IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
    }

    public Task<Result<string>> GetTokenAsync(CancellationToken cancellationToken)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(token.Token);

        if (jwtToken.ValidTo == default)
        {
            return new Result<string>(token.Token).ToTaskResult();
        }

        DateTimeOffset expires = jwtToken.ValidTo;

        if (expires > DateTimeOffset.Now)
        {
            return new Result<string>(token.Token).ToTaskResult();
        }

        cancellationToken.ThrowIfCancellationRequested();

        return authenticationService.RefreshTokenAsync(token.RefreshToken, cancellationToken)
            .IfSuccessAsync(
                value =>
                {
                    token = value;

                    return token.Token.ToResult();
                }
            );
    }

    public Task<Result> LoginAsync(User user, CancellationToken cancellationToken)
    {
        return authenticationService.LoginAsync(user, cancellationToken)
            .IfSuccessAsync<TokenResult>(value => token = value);
    }

    public Task<Result> LoginAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return authenticationService.RefreshTokenAsync(refreshToken, cancellationToken)
            .IfSuccessAsync<TokenResult>(value => token = value);
    }
}
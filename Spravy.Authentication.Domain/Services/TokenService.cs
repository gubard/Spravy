using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Extensions;
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

    public ConfiguredValueTaskAwaitable<Result<string>> GetTokenAsync(CancellationToken ct)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(token.Token);

        if (jwtToken.ValidTo == default)
        {
            return new Result<string>(token.Token).ToValueTaskResult().ConfigureAwait(false);
        }

        DateTimeOffset expires = jwtToken.ValidTo;

        if (expires > DateTimeOffset.Now)
        {
            return new Result<string>(token.Token).ToValueTaskResult().ConfigureAwait(false);
        }

        return authenticationService.RefreshTokenAsync(token.RefreshToken, ct)
           .IfSuccessAsync(value =>
            {
                token = value;

                return token.Token.ToResult().ToValueTaskResult().ConfigureAwait(false);
            }, ct);
    }

    public ConfiguredValueTaskAwaitable<Result> LoginAsync(User user, CancellationToken ct)
    {
        return authenticationService.LoginAsync(user, ct)
           .IfSuccessAsync(value =>
            {
                token = value;

                return Result.AwaitableSuccess;
            }, ct);
    }

    public ConfiguredValueTaskAwaitable<Result> LoginAsync(string refreshToken, CancellationToken ct)
    {
        return authenticationService.RefreshTokenAsync(refreshToken, ct)
           .IfSuccessAsync(value =>
            {
                token = value;

                return Result.AwaitableSuccess;
            }, ct);
    }
}
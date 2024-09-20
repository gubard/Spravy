namespace Spravy.Authentication.Service.Services;

public class TokenService : ITokenService
{
    private readonly ITokenFactory tokenFactory;
    private TokenResult token;

    public TokenService(ITokenFactory tokenFactory)
    {
        this.tokenFactory = tokenFactory;

        token = tokenFactory
            .Create(new("authentication.service", Guid.Empty, Role.Service, string.Empty))
            .ThrowIfError();
    }

    public ConfiguredValueTaskAwaitable<Result<string>> GetTokenAsync(CancellationToken ct)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(token.Token);
        DateTimeOffset expires = jwtToken.ValidTo;

        if (expires > DateTimeOffset.Now)
        {
            return new Result<string>(token.Token).ToValueTaskResult().ConfigureAwait(false);
        }

        return tokenFactory
            .Create(new("authentication.service", Guid.Empty, Role.Service, string.Empty))
            .IfSuccessAsync(
                t =>
                {
                    token = t;

                    return new Result<string>(token.Token)
                        .ToValueTaskResult()
                        .ConfigureAwait(false);
                },
                ct
            );
    }

    public Cvtar LoginAsync(User user, CancellationToken ct)
    {
        throw new NotSupportedException();
    }

    public Cvtar LoginAsync(string refreshToken, CancellationToken ct)
    {
        throw new NotSupportedException();
    }
}

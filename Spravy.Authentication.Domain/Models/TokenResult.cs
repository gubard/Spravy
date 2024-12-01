namespace Spravy.Authentication.Domain.Models;

public readonly struct TokenResult
{
    public TokenResult(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
    }

    public string Token { get; }
    public string RefreshToken { get; }
}
namespace Spravy.Authentication.Domain.Models;

public  readonly struct TokenResult
{
    public TokenResult(string token)
    {
        Token = token;
    }

    public string Token { get; }
}
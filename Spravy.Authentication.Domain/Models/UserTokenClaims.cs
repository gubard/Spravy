namespace Spravy.Authentication.Domain.Models;

public struct UserTokenClaims
{
    public UserTokenClaims(string login)
    {
        Login = login;
    }

    public string Login { get; }
}
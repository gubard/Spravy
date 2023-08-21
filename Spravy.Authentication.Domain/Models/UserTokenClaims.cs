namespace Spravy.Authentication.Domain.Models;

public struct UserTokenClaims
{
    public UserTokenClaims(string login, Guid id)
    {
        Login = login;
        Id = id;
    }

    public string Login { get; }
    public Guid Id { get; }
}
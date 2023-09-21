using Spravy.Domain.Enums;

namespace Spravy.Authentication.Domain.Models;

public struct TokenClaims
{
    public TokenClaims(string login, Guid id, Role role)
    {
        Login = login;
        Id = id;
        Role = role;
    }

    public string Login { get; }
    public Guid Id { get; }
    public Role Role { get; }
}
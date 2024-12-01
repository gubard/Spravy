namespace Spravy.Authentication.Domain.Models;

public struct UserTokenClaims
{
    public UserTokenClaims(string login, Guid id, Role role, string email)
    {
        Login = login;
        Id = id;
        Role = role;
        Email = email;
    }

    public string Login { get; }
    public Guid Id { get; }
    public Role Role { get; }
    public string Email { get; }
}
namespace Spravy.Domain.Models;

public readonly struct User
{
    public User(string login, string password)
    {
        Login = login;
        Password = password;
    }

    public string Login { get; }
    public string Password { get; }
}
namespace Spravy.Authentication.Domain.Models;

public readonly struct CreateUserOptions
{
    public CreateUserOptions(string login, string password)
    {
        Login = login;
        Password = password;
    }

    public string Login { get; }
    public string Password { get; }
}
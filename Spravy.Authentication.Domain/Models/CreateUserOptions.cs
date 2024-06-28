namespace Spravy.Authentication.Domain.Models;

public readonly struct CreateUserOptions
{
    public CreateUserOptions(string login, string password, string email)
    {
        Login = login;
        Password = password;
        Email = email;
    }

    public string Login { get; }
    public string Password { get; }
    public string Email { get; }
}

namespace Spravy.Tests.Models;

public class ImapConnection
{
    public ImapConnection(string host, string login, string password)
    {
        Host = host;
        Login = login;
        Password = password;
    }

    public string Host { get; }
    public string Login { get; }
    public string Password { get; }
}
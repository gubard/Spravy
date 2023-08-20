namespace Spravy.Authentication.Domain.Models;

public readonly struct HashOptions
{
    public HashOptions(string salt, string hashMethod)
    {
        Salt = salt;
        HashMethod = hashMethod;
    }

    public string Salt { get; }
    public string HashMethod { get; }
}
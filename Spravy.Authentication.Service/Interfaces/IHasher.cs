namespace Spravy.Authentication.Service.Interfaces;

public interface IHasher
{
    string HashMethod { get; }
    string ComputeHash(string input);
}

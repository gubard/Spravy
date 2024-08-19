namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IPasswordGenerator
{
    Result<string> GeneratePassword(string key, GeneratePasswordOptions passwordOptions);
}

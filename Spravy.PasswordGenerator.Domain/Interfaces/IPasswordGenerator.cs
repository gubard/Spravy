using Spravy.PasswordGenerator.Domain.Models;

namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IPasswordGenerator
{
    string GeneratePassword(string key, GeneratePasswordOptions options, CancellationToken cancellationToken);
}
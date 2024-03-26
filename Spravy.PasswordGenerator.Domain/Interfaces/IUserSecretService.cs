namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IUserSecretService
{
    Task<string> GetUserSecretAsync(CancellationToken cancellationToken);
}
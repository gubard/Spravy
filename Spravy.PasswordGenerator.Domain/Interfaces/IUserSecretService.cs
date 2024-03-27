namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IUserSecretService
{
    Task<byte[]> GetUserSecretAsync(CancellationToken cancellationToken);
}
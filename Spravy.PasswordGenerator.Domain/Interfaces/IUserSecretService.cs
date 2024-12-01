namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IUserSecretService
{
    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<byte>>> GetUserSecretAsync(CancellationToken ct);
}
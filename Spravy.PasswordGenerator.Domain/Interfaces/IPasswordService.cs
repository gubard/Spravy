namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IPasswordService
{
    Cvtar AddPasswordItemAsync(AddPasswordOptions options, CancellationToken ct);
    Cvtar EditPasswordItemsAsync(EditPasswordItems options, CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result<PasswordItem>> GetPasswordItemAsync(Guid id, CancellationToken ct);
    Cvtar DeletePasswordItemAsync(Guid id, CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result<string>> GeneratePasswordAsync(Guid id, CancellationToken ct);
}
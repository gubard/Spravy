namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IPasswordService
{
    Cvtar AddPasswordItemAsync(AddPasswordOptions options, CancellationToken ct);
    Cvtar UpdatePasswordItemNameAsync(Guid id, string name, CancellationToken ct);
    Cvtar UpdatePasswordItemKeyAsync(Guid id, string key, CancellationToken ct);
    Cvtar UpdatePasswordItemLengthAsync(Guid id, ushort length, CancellationToken ct);
    Cvtar UpdatePasswordItemRegexAsync(Guid id, string regex, CancellationToken ct);
    Cvtar UpdatePasswordItemLoginAsync(Guid id, string login, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<PasswordItem>> GetPasswordItemAsync(Guid id, CancellationToken ct);

    Cvtar DeletePasswordItemAsync(Guid id, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<string>> GeneratePasswordAsync(Guid id, CancellationToken ct);

    Cvtar UpdatePasswordItemIsAvailableNumberAsync(Guid id, bool isAvailableNumber, CancellationToken ct);

    Cvtar UpdatePasswordItemIsAvailableLowerLatinAsync(Guid id, bool isAvailableLowerLatin, CancellationToken ct);

    Cvtar UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken ct
    );

    Cvtar UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
        CancellationToken ct
    );

    Cvtar UpdatePasswordItemIsAvailableUpperLatinAsync(Guid id, bool isAvailableUpperLatin, CancellationToken ct);
}
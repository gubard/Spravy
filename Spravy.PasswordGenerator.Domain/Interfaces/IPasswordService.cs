using Spravy.PasswordGenerator.Domain.Models;

namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IPasswordService
{
    Task AddPasswordItemAsync(AddPasswordOptions options, CancellationToken cancellationToken);
    Task<IEnumerable<PasswordItem>> GetPasswordItemsAsync(CancellationToken cancellationToken);
    Task<PasswordItem> GetPasswordItemAsync(Guid id, CancellationToken cancellationToken);
    Task DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken);
    Task<string> GeneratePasswordAsync(Guid id, CancellationToken cancellationToken);
    Task UpdatePasswordItemNameAsync(Guid id, string name, CancellationToken cancellationToken);
    Task UpdatePasswordItemKeyAsync(Guid id, string key, CancellationToken cancellationToken);
    Task UpdatePasswordItemLengthAsync(Guid id, ushort length, CancellationToken cancellationToken);
    Task UpdatePasswordItemRegexAsync(Guid id, string regex, CancellationToken cancellationToken);
    Task UpdatePasswordItemIsAvailableNumberAsync(Guid id, bool isAvailableNumber, CancellationToken cancellationToken);

    Task UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
        CancellationToken cancellationToken
    );

    Task UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken cancellationToken
    );

    Task UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
        CancellationToken cancellationToken
    );

    Task UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
        CancellationToken cancellationToken
    );
}
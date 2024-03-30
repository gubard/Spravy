using Spravy.Domain.Models;
using Spravy.PasswordGenerator.Domain.Models;

namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IPasswordService
{
    Task<Result> AddPasswordItemAsync(AddPasswordOptions options, CancellationToken cancellationToken);
    Task<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(CancellationToken cancellationToken);
    Task<Result<PasswordItem>> GetPasswordItemAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<string>> GeneratePasswordAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> UpdatePasswordItemNameAsync(Guid id, string name, CancellationToken cancellationToken);
    Task<Result> UpdatePasswordItemKeyAsync(Guid id, string key, CancellationToken cancellationToken);
    Task<Result> UpdatePasswordItemLengthAsync(Guid id, ushort length, CancellationToken cancellationToken);
    Task<Result> UpdatePasswordItemRegexAsync(Guid id, string regex, CancellationToken cancellationToken);

    Task<Result> UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
        CancellationToken cancellationToken
    );

    Task<Result> UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
        CancellationToken cancellationToken
    );

    Task<Result> UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken cancellationToken
    );

    Task<Result> UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
        CancellationToken cancellationToken
    );

    Task<Result> UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
        CancellationToken cancellationToken
    );
}
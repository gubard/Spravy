using Spravy.Domain.Models;
using Spravy.PasswordGenerator.Domain.Models;

namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IPasswordService
{
    ValueTask<Result> AddPasswordItemAsync(AddPasswordOptions options, CancellationToken cancellationToken);
    ValueTask<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(CancellationToken cancellationToken);
    ValueTask<Result<PasswordItem>> GetPasswordItemAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result> DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result<string>> GeneratePasswordAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result> UpdatePasswordItemNameAsync(Guid id, string name, CancellationToken cancellationToken);
    ValueTask<Result> UpdatePasswordItemKeyAsync(Guid id, string key, CancellationToken cancellationToken);
    ValueTask<Result> UpdatePasswordItemLengthAsync(Guid id, ushort length, CancellationToken cancellationToken);
    ValueTask<Result> UpdatePasswordItemRegexAsync(Guid id, string regex, CancellationToken cancellationToken);

    ValueTask<Result> UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
        CancellationToken cancellationToken
    );

    ValueTask<Result> UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
        CancellationToken cancellationToken
    );

    ValueTask<Result> UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken cancellationToken
    );

    ValueTask<Result> UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
        CancellationToken cancellationToken
    );

    ValueTask<Result> UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
        CancellationToken cancellationToken
    );
}
using System.Runtime.CompilerServices;
using Spravy.Domain.Models;
using Spravy.PasswordGenerator.Domain.Models;

namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IPasswordService
{
    ConfiguredValueTaskAwaitable<Result> AddPasswordItemAsync(
        AddPasswordOptions options,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<PasswordItem>> GetPasswordItemAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result<string>> GeneratePasswordAsync(Guid id, CancellationToken cancellationToken);

    ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemNameAsync(
        Guid id,
        string name,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemKeyAsync(
        Guid id,
        string key,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemLengthAsync(
        Guid id,
        ushort length,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemRegexAsync(
        Guid id,
        string regex,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
        CancellationToken cancellationToken
    );
}
namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IPasswordService
{
    Cvtar AddPasswordItemAsync(AddPasswordOptions options, CancellationToken ct);
    Cvtar EditPasswordItemsAsync(EditPasswordItems options, CancellationToken ct);
    Cvtar DeletePasswordItemAsync(Guid id, CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result<string>> GeneratePasswordAsync(Guid id, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenPasswordItemIdsAsync(
        OptionStruct<Guid> id,
        CancellationToken ct
    );

    ConfiguredCancelableAsyncEnumerable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        CancellationToken ct
    );
}
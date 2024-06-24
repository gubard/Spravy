namespace Spravy.Domain.Interfaces;

public interface IObjectStorage
{
    ConfiguredValueTaskAwaitable<Result<bool>> IsExistsAsync(string id, CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result> DeleteAsync(string id, CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result> SaveObjectAsync(string id, object obj, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<TObject>> GetObjectAsync<TObject>(
        string id,
        CancellationToken ct
    ) where TObject : notnull;
}
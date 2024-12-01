namespace Spravy.Domain.Interfaces;

public interface IObjectStorage
{
    ConfiguredValueTaskAwaitable<Result<bool>> IsExistsAsync(string id, CancellationToken ct);
    Cvtar DeleteAsync(string id, CancellationToken ct);
    Cvtar SaveObjectAsync(string id, object obj, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<TObject>> GetObjectAsync<TObject>(string id, CancellationToken ct)
        where TObject : notnull;
}
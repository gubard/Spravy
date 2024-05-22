namespace Spravy.Domain.Interfaces;

public interface IObjectStorage
{
    ConfiguredValueTaskAwaitable<Result<bool>> IsExistsAsync(string id);
    ConfiguredValueTaskAwaitable<Result> DeleteAsync(string id);
    ConfiguredValueTaskAwaitable<Result> SaveObjectAsync(string id, object obj);
    ConfiguredValueTaskAwaitable<Result<TObject>> GetObjectAsync<TObject>(string id);
}
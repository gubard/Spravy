namespace Spravy.Domain.Interfaces;

public interface IObjectStorage
{
    Task<bool> IsExistsAsync(string id);
    Task DeleteAsync(string id);
    Task SaveObjectAsync(string id, object obj);
    Task<TObject> GetObjectAsync<TObject>(string id);
}
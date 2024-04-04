using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IObjectStorage
{
    ValueTask<Result<bool>> IsExistsAsync(string id);
    ValueTask<Result> DeleteAsync(string id);
    ValueTask<Result> SaveObjectAsync(string id, object obj);
    ValueTask<Result<TObject>> GetObjectAsync<TObject>(string id);
}
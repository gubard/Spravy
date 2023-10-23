namespace Spravy.Domain.Interfaces;

public interface ISerializer
{
    Task SerializeAsync(object obj, Stream stream);
    Task<TObject> DeserializeAsync<TObject>(Stream stream);
}
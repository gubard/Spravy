namespace Spravy.Domain.Interfaces;

public interface ISerializer
{
    ConfiguredValueTaskAwaitable<Result> SerializeAsync<T>(T obj, Stream stream);
    ConfiguredValueTaskAwaitable<Result<TObject>> DeserializeAsync<TObject>(Stream stream) where TObject : notnull;
    Result<TObject> Deserialize<TObject>(Stream stream) where TObject : notnull;
}
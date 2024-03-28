namespace Spravy.Domain.Interfaces;

public interface ISerializer
{
    void Serialize(object obj, Stream stream);
    TObject Deserialize<TObject>(Stream stream);
}
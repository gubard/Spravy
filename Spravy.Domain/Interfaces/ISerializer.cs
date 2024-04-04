using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface ISerializer
{
    ValueTask<Result> Serialize(object obj, Stream stream);
    ValueTask<Result<TObject>> Deserialize<TObject>(Stream stream);
}
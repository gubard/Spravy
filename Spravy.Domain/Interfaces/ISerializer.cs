using System.Runtime.CompilerServices;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface ISerializer
{
    ConfiguredValueTaskAwaitable<Result> SerializeAsync(object obj, Stream stream);
    ConfiguredValueTaskAwaitable<Result<TObject>> DeserializeAsync<TObject>(Stream stream);
    Result<TObject> Deserialize<TObject>(Stream stream);
}
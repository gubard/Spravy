using System.Runtime.CompilerServices;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface ISerializer
{
    ConfiguredValueTaskAwaitable<Result> Serialize(object obj, Stream stream);
    ConfiguredValueTaskAwaitable<Result<TObject>> Deserialize<TObject>(Stream stream);
}
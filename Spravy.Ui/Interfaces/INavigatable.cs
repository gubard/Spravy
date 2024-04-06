using System.Runtime.CompilerServices;
using Spravy.Domain.Models;

namespace Spravy.Ui.Interfaces;

public interface INavigatable : ISaveState
{
    bool IsPooled { get; }
    string ViewId { get; }

    Result Stop();
    ConfiguredValueTaskAwaitable<Result> SetStateAsync(object setting);
}
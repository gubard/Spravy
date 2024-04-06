using System.Runtime.CompilerServices;
using Spravy.Domain.Models;

namespace Spravy.Ui.Interfaces;

public interface ISaveState
{
    ConfiguredValueTaskAwaitable<Result> SaveStateAsync();
}
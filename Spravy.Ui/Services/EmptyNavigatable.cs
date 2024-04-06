using System.Runtime.CompilerServices;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class EmptyNavigatable : INavigatable
{
    public bool IsPooled => false;
    public string ViewId => TypeCache<EmptyNavigatable>.Type.Name;

    public ConfiguredValueTaskAwaitable<Result> SaveStateAsync()
    {
        return Result.AwaitableFalse;
    }

    public Result Stop()
    {
        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> SetStateAsync(object setting)
    {
        return Result.AwaitableFalse;
    }
}
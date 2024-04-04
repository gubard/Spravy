using System.Threading.Tasks;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class EmptyNavigatable : INavigatable
{
    public bool IsPooled => false;
    public string ViewId => TypeCache<EmptyNavigatable>.Type.Name;

    public ValueTask<Result> SaveStateAsync()
    {
        return Result.SuccessValueTask;
    }

    public Result Stop()
    {
        return Result.Success;
    }

    public ValueTask<Result> SetStateAsync(object setting)
    {
        return Result.SuccessValueTask;
    }
}
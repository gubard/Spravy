namespace Spravy.Ui.Services;

public class EmptyNavigatable : INavigatable
{
    public bool IsPooled
    {
        get => false;
    }

    public string ViewId
    {
        get => TypeCache<EmptyNavigatable>.Type.Name;
    }

    public ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Result Stop()
    {
        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> SetStateAsync(object setting, CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}

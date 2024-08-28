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

    public Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Result Stop()
    {
        return Result.Success;
    }

    public Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}

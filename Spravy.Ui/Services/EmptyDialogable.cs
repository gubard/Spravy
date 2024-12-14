namespace Spravy.Ui.Services;

public class EmptyDialogable : IDialogable
{
    public string ViewId => TypeCache<EmptyDialogable>.Name;

    public Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
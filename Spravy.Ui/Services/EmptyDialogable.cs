namespace Spravy.Ui.Services;

public class EmptyDialogable : IDialogable
{
    public string ViewId => $"{TypeCache<EmptyDialogable>.Type}";

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
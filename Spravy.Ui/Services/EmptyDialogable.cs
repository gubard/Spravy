namespace Spravy.Ui.Services;

public class EmptyDialogable : IDialogable
{
    public string ViewId
    {
        get => $"{TypeCache<EmptyDialogable>.Type}";
    }

    public Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}

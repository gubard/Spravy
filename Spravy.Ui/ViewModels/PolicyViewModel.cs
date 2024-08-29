namespace Spravy.Ui.ViewModels;

public class PolicyViewModel : NavigatableViewModelBase
{
    public PolicyViewModel() : base(true)
    {
    }

    public override string ViewId
    {
        get => $"{TypeCache<PolicyViewModel>.Type}";
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
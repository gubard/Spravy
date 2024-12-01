namespace Spravy.Ui.ViewModels;

public class PolicyViewModel : NavigatableViewModelBase
{
    public PolicyViewModel(
        Application application,
        IClipboardService clipboardService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    ) : base(true)
    {
        CopyPolicyCommand = SpravyCommand.Create(
            ct => clipboardService.SetTextAsync(application.GetResource("PolicyView.Policy")?.ToString(), ct),
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand CopyPolicyCommand { get; }
    public override string ViewId => $"{TypeCache<PolicyViewModel>.Type}";

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
namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class PasswordItemSettingsViewModel : DialogableViewModelBase
{
    public PasswordItemSettingsViewModel(PasswordItemEntityNotify item)
    {
        Item = item;
    }

    public PasswordItemEntityNotify Item { get; }

    public override string ViewId
    {
        get => $"{TypeCache<PasswordItemSettingsViewModel>.Type}";
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

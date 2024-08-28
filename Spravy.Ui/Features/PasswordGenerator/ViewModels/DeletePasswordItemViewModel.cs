namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class DeletePasswordItemViewModel : DialogableViewModelBase
{
    public DeletePasswordItemViewModel(PasswordItemEntityNotify item)
    {
        Item = item;
    }

    public PasswordItemEntityNotify Item { get; }

    public override string ViewId
    {
        get => $"{TypeCache<DeletePasswordItemViewModel>.Type}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}

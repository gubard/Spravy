namespace Spravy.Ui.Models;

public abstract class DialogableViewModelBase : ViewModelBase, IDialogable
{
    public abstract string ViewId { get; }

    public abstract Cvtar LoadStateAsync(CancellationToken ct);
    public abstract Cvtar SaveStateAsync(CancellationToken ct);
}
namespace Spravy.Ui.Models;

public abstract class NavigatableViewModelBase(bool isPooled) : ViewModelBase, INavigatable
{
    public bool IsPooled { get; } = isPooled;
    public abstract string ViewId { get; }

    public abstract Result Stop();
    public abstract Cvtar LoadStateAsync(CancellationToken ct);
    public abstract Cvtar SaveStateAsync(CancellationToken ct);
}

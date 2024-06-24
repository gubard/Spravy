namespace Spravy.Ui.Models;

public abstract class NavigatableViewModelBase : ViewModelBase, INavigatable
{
    protected NavigatableViewModelBase(bool isPooled)
    {
        IsPooled = isPooled;
    }
    
    public bool IsPooled { get; }
    public abstract string ViewId { get; }
    
    public abstract Result Stop();
    public abstract ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct);
    
    public abstract ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    );
}
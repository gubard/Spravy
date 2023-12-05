using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Models;

public abstract class NavigatableViewModelBase : ViewModelBase, INavigatable
{
    public NavigatableViewModelBase(bool isPooled)
    {
        IsPooled = isPooled;
    }

    public bool IsPooled { get; }

    public abstract void Stop();
}
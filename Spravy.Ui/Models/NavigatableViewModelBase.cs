using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Models;

public class NavigatableViewModelBase : ViewModelBase, INavigatable
{
    public NavigatableViewModelBase(bool isPooled)
    {
        IsPooled = isPooled;
    }

    public bool IsPooled { get; }
}
using System.Threading.Tasks;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Models;

public abstract class NavigatableViewModelBase : ViewModelBase, INavigatable
{
    public NavigatableViewModelBase(bool isPooled)
    {
        IsPooled = isPooled;
    }

    public bool IsPooled { get; }
    public abstract string ViewId { get; }

    public abstract void Stop();
    public abstract Task SaveStateAsync();
    public abstract Task SetStateAsync(object setting);
}
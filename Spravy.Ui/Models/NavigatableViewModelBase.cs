using System.Runtime.CompilerServices;
using System.Threading;
using Spravy.Domain.Models;
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

    public abstract Result Stop();
    public abstract ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken);
    public abstract ConfiguredValueTaskAwaitable<Result> SetStateAsync(object setting, CancellationToken cancellationToken);
}
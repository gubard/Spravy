using System;
using System.Threading;
using System.Threading.Tasks;

namespace Spravy.Ui.Interfaces;

public interface INavigator
{
    Task<INavigatable?> NavigateBackAsync(CancellationToken cancellationToken);

    Task NavigateToAsync<TViewModel>(TViewModel parameter, CancellationToken cancellationToken)
        where TViewModel : INavigatable;

    Task NavigateToAsync(Type type, CancellationToken cancellationToken);

    Task NavigateToAsync<TViewModel>(Action<TViewModel> setup, CancellationToken cancellationToken)
        where TViewModel : INavigatable;
}
using System;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;

namespace Spravy.Ui.Interfaces;

public interface INavigator
{
    Task<IRoutableViewModel?> NavigateBackAsync();
    Task NavigateToAsync<TViewModel>(TViewModel parameter, CancellationToken cancellationToken) where TViewModel : IRoutableViewModel;
    Task NavigateToAsync(Type type, CancellationToken cancellationToken);

    Task NavigateToAsync<TViewModel>(Action<TViewModel> setup, CancellationToken cancellationToken)
        where TViewModel : IRoutableViewModel;
}
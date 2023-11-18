using System;
using System.Threading.Tasks;
using ReactiveUI;

namespace Spravy.Ui.Interfaces;

public interface INavigator
{
    IObservable<IRoutableViewModel?> NavigateBack();
    Task NavigateToAsync<TViewModel>(TViewModel parameter) where TViewModel : IRoutableViewModel;
    Task NavigateToAsync(Type type);

    Task NavigateToAsync<TViewModel>(Action<TViewModel>? setup = null)
        where TViewModel : IRoutableViewModel;
}
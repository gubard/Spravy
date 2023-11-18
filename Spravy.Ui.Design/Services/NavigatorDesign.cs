using ReactiveUI;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Design.Services;

public class NavigatorDesign : INavigator
{
    public IObservable<IRoutableViewModel?> NavigateBack()
    {
        throw new NotImplementedException();
    }

    public Task NavigateToAsync<TViewModel>(TViewModel parameter) where TViewModel : IRoutableViewModel
    {
        throw new NotImplementedException();
    }

    public Task NavigateToAsync(Type type)
    {
        throw new NotImplementedException();
    }

    public Task NavigateToAsync<TViewModel>(Action<TViewModel>? setup = null) where TViewModel : IRoutableViewModel
    {
        throw new NotImplementedException();
    }
}
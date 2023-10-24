using System;
using ReactiveUI;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Design;

public class NavigatorDesign : INavigator
{
    public IObservable<IRoutableViewModel?> NavigateBack()
    {
        throw new NotImplementedException();
    }

    public IObservable<IRoutableViewModel> NavigateTo<TViewModel>(TViewModel parameter)
        where TViewModel : IRoutableViewModel
    {
        throw new NotImplementedException();
    }

    public IObservable<IRoutableViewModel> NavigateTo(Type type)
    {
        throw new NotImplementedException();
    }

    public IObservable<IRoutableViewModel> NavigateTo<TViewModel>(Action<TViewModel>? setup = null)
        where TViewModel : IRoutableViewModel
    {
        throw new NotImplementedException();
    }
}
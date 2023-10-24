using System;
using ReactiveUI;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Design;

public class NavigatorDesign : INavigator
{
    public IObservable<IRoutableViewModel?> NavigateBack()
    {
        return null;
    }

    public IObservable<IRoutableViewModel> NavigateTo<TViewModel>(TViewModel parameter) where TViewModel : IRoutableViewModel
    {
        return null;
    }

    public IObservable<IRoutableViewModel> NavigateTo(Type type)
    {
        return null;
    }

    public IObservable<IRoutableViewModel> NavigateTo<TViewModel>(Action<TViewModel>? setup = null) where TViewModel : IRoutableViewModel
    {
        return null;
    }
}
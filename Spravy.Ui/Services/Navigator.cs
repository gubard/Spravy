using System;
using Ninject;
using ReactiveUI;
using Serilog;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.Services;

public class Navigator : INavigator
{
    [Inject]
    public required RoutingState RoutingState { get; init; }

    [Inject]
    public required IKernel Resolver { get; init; }

    [Inject]
    public required AppConfiguration Configuration { get; init; }

    public IObservable<IRoutableViewModel?> NavigateBack()
    {
        return NavigateTo(Configuration.DefaultMainViewType);
    }

    public IObservable<IRoutableViewModel> NavigateTo(Type type)
    {
        var viewModel = (IRoutableViewModel)Resolver.Get(type);

        return RoutingState.Navigate.Execute(viewModel);
    }

    public IObservable<IRoutableViewModel> NavigateTo<TViewModel>(Action<TViewModel>? setup = null)
        where TViewModel : IRoutableViewModel
    {
        var viewModel = Resolver.Get<TViewModel>();

        if (setup is null)
        {
            return RoutingState.Navigate.Execute(viewModel);
        }

        setup.Invoke(viewModel);

        return RoutingState.Navigate.Execute(viewModel);
    }

    public IObservable<IRoutableViewModel> NavigateTo<TViewModel>(TViewModel parameter)
        where TViewModel : IRoutableViewModel
    {
        return RoutingState.Navigate.Execute(parameter);
    }
}
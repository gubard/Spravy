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
        Log.Logger.Information("Test {Number}", 10);
        var viewModel = Resolver.Get<TViewModel>();
        Log.Logger.Information("Test {Number}", 11);

        if (setup is null)
        {
            Log.Logger.Information("Test {Number}", 12);
            return RoutingState.Navigate.Execute(viewModel);
        }

        Log.Logger.Information("Test {Number}", 13);
        setup.Invoke(viewModel);
        Log.Logger.Information("Test {Number}", 14);

        return RoutingState.Navigate.Execute(viewModel);
    }

    public IObservable<IRoutableViewModel> NavigateTo<TViewModel>(TViewModel parameter)
        where TViewModel : IRoutableViewModel
    {
        return RoutingState.Navigate.Execute(parameter);
    }
}
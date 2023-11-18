using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Ninject;
using ReactiveUI;
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
        throw new NotSupportedException();
    }

    public async Task NavigateToAsync(Type type)
    {
        var viewModel = (IRoutableViewModel)Resolver.Get(type);
        await Dispatcher.UIThread.InvokeAsync(() => RoutingState.Navigate.Execute(viewModel));
    }

    public async Task NavigateToAsync<TViewModel>(Action<TViewModel>? setup = null)
        where TViewModel : IRoutableViewModel
    {
        var viewModel = Resolver.Get<TViewModel>();

        if (setup is null)
        {
            await Dispatcher.UIThread.InvokeAsync(() => RoutingState.Navigate.Execute(viewModel));
            
            return;
        }

        setup.Invoke(viewModel);
        await Dispatcher.UIThread.InvokeAsync(() => RoutingState.Navigate.Execute(viewModel));
    }

    public async Task NavigateToAsync<TViewModel>(TViewModel parameter)
        where TViewModel : IRoutableViewModel
    {
        await Dispatcher.UIThread.InvokeAsync(() => RoutingState.Navigate.Execute(parameter));
    }
}
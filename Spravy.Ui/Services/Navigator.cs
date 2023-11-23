using System;
using System.Threading;
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

    public async Task NavigateToAsync(Type type, CancellationToken cancellationToken)
    {
        var viewModel = (IRoutableViewModel)Resolver.Get(type);
        await Dispatcher.UIThread.InvokeAsync(() => RoutingState.Navigate.Execute(viewModel));
    }

    public async Task NavigateToAsync<TViewModel>(Action<TViewModel> setup, CancellationToken cancellationToken)
        where TViewModel : IRoutableViewModel
    {
        var viewModel = Resolver.Get<TViewModel>();
        await Dispatcher.UIThread.InvokeAsync(() => setup.Invoke(viewModel));
        await Dispatcher.UIThread.InvokeAsync(() => RoutingState.Navigate.Execute(viewModel));
    }

    public async Task NavigateToAsync<TViewModel>(TViewModel parameter, CancellationToken cancellationToken)
        where TViewModel : IRoutableViewModel
    {
        await Dispatcher.UIThread.InvokeAsync(() => RoutingState.Navigate.Execute(parameter));
    }
}
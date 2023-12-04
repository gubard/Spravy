using System;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class Navigator : INavigator
{
    private readonly QueryList<IRoutableViewModel> list = new(20);

    [Inject]
    public required IKernel Resolver { get; init; }

    [Inject]
    public required IContent Content { get; init; }

    public async Task NavigateToAsync(Type type, CancellationToken cancellationToken)
    {
        var viewModel = (IRoutableViewModel)Resolver.Get(type);
        await this.InvokeUIAsync(() => Content.Content = viewModel);
        list.Add(viewModel);
    }

    public async Task NavigateToAsync<TViewModel>(Action<TViewModel> setup, CancellationToken cancellationToken)
        where TViewModel : IRoutableViewModel
    {
        var viewModel = Resolver.Get<TViewModel>();

        await this.InvokeUIAsync(
            () =>
            {
                setup.Invoke(viewModel);
                Content.Content = viewModel;
            }
        );

        list.Add(viewModel);
    }

    public async Task<IRoutableViewModel?> NavigateBackAsync(CancellationToken cancellationToken)
    {
        var viewModel = list.Pop();

        if (viewModel is null)
        {
            return null;
        }
        
        await this.InvokeUIAsync(() => Content.Content = viewModel);

        return viewModel;
    }

    public async Task NavigateToAsync<TViewModel>(TViewModel parameter, CancellationToken cancellationToken)
        where TViewModel : IRoutableViewModel
    {
        await this.InvokeUIAsync(() => Content.Content = parameter);
        list.Add(parameter);
    }
}
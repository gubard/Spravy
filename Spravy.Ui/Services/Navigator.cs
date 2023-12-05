using System;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using Spravy.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class Navigator : INavigator
{
    private readonly QueryList<INavigatable> list = new(20);

    [Inject]
    public required IKernel Resolver { get; init; }

    [Inject]
    public required IContent Content { get; init; }

    private void AddCurrentContent()
    {
        if (Content.Content is null)
        {
            return;
        }

        var content = (INavigatable)Content.Content;
        content.Stop();

        if (!content.IsPooled)
        {
            return;
        }

        list.Add(content);
    }

    public async Task NavigateToAsync(Type type, CancellationToken cancellationToken)
    {
        AddCurrentContent();
        var viewModel = (INavigatable)Resolver.Get(type);
        await this.InvokeUIAsync(() => Content.Content = viewModel);
    }

    public async Task NavigateToAsync<TViewModel>(Action<TViewModel> setup, CancellationToken cancellationToken)
        where TViewModel : INavigatable
    {
        AddCurrentContent();
        var viewModel = Resolver.Get<TViewModel>();

        await this.InvokeUIAsync(
            () =>
            {
                setup.Invoke(viewModel);
                Content.Content = viewModel;
            }
        );
    }

    public async Task<INavigatable?> NavigateBackAsync(CancellationToken cancellationToken)
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
        where TViewModel : INavigatable
    {
        AddCurrentContent();
        await this.InvokeUIAsync(() => Content.Content = parameter);
    }
}
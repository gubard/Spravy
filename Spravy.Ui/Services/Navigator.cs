using System;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Services;

public class Navigator : INavigator
{
    private Action<object> lastSetup = ActionHelper<object>.Empty;
    private readonly QueryList<NavigatorItem> list = new(20);

    [Inject]
    public required IKernel Resolver { get; init; }

    [Inject]
    public required IContent Content { get; init; }

    [Inject]
    public required IDialogViewer DialogViewer { get; init; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    private async Task AddCurrentContentAsync(Action<object> setup)
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

        await content.SaveStateAsync().ConfigureAwait(false);
        list.Add(new NavigatorItem(content, lastSetup));
        lastSetup = setup;
    }

    public async Task NavigateToAsync(Type type, CancellationToken cancellationToken)
    {
        await AddCurrentContentAsync(ActionHelper<object>.Empty);
        var viewModel = (INavigatable)Resolver.Get(type);
        await this.InvokeUIAsync(() => Content.Content = viewModel);
    }

    public async Task NavigateToAsync<TViewModel>(Action<TViewModel> setup, CancellationToken cancellationToken)
        where TViewModel : INavigatable
    {
        await AddCurrentContentAsync(obj => setup.Invoke((TViewModel)obj));

        if (Content.Content is IRefresh refresh && Content.Content is TViewModel vm)
        {
            await this.InvokeUIAsync(() => setup.Invoke(vm));
            await refresh.RefreshAsync(cancellationToken);
        }
        else
        {
            var viewModel = Resolver.Get<TViewModel>();

            await this.InvokeUIAsync(
                () =>
                {
                    setup.Invoke(viewModel);
                    Content.Content = viewModel;
                }
            );
        }
    }

    public async Task NavigateToAsync<TViewModel>(CancellationToken cancellationToken) where TViewModel : INavigatable
    {
        await AddCurrentContentAsync(ActionHelper<object>.Empty);
        var viewModel = Resolver.Get<TViewModel>();
        await this.InvokeUIAsync(() => Content.Content = viewModel);
    }

    public async Task<INavigatable?> NavigateBackAsync(CancellationToken cancellationToken)
    {
        var item = list.Pop();

        if (item is null)
        {
            return null;
        }

        if (await DialogViewer.CloseLastDialogAsync(cancellationToken))
        {
            return new EmptyNavigatable();
        }

        if (MainSplitViewModel.IsPaneOpen)
        {
            MainSplitViewModel.IsPaneOpen = false;

            return new EmptyNavigatable();
        }

        await this.InvokeUIAsync(
            async () =>
            {
                item.Setup.Invoke(item.Navigatable);
                Content.Content = item.Navigatable;

                if (item.Navigatable is IRefresh refresh)
                {
                    await refresh.RefreshAsync(cancellationToken);
                }
            }
        );

        return item.Navigatable;
    }

    public async Task NavigateToAsync<TViewModel>(TViewModel parameter, CancellationToken cancellationToken)
        where TViewModel : INavigatable
    {
        await AddCurrentContentAsync(ActionHelper<object>.Empty);
        await this.InvokeUIAsync(() => Content.Content = parameter);
    }
}
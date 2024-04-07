using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Ninject;
using Spravy.Domain.Extensions;
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

    private ConfiguredValueTaskAwaitable<Result> AddCurrentContentAsync(Action<object> setup)
    {
        if (Content.Content is null)
        {
            return Result.AwaitableFalse;
        }

        var content = (INavigatable)Content.Content;
        content.Stop();

        if (!content.IsPooled)
        {
            return Result.AwaitableFalse;
        }

        return content.SaveStateAsync()
            .IfSuccessAsync(
                () =>
                {
                    list.Add(new NavigatorItem(content, lastSetup));
                    lastSetup = setup;

                    return Result.AwaitableFalse;
                }
            );
    }

    public ConfiguredValueTaskAwaitable<Result> NavigateToAsync(Type type, CancellationToken cancellationToken)
    {
        var viewModel = (INavigatable)Resolver.Get(type);

        return AddCurrentContentAsync(ActionHelper<object>.Empty)
            .IfSuccessAsync(() => this.InvokeUIBackgroundAsync(() => Content.Content = viewModel));
    }

    public ConfiguredValueTaskAwaitable<Result> NavigateToAsync<TViewModel>(
        Action<TViewModel> setup,
        CancellationToken cancellationToken
    )
        where TViewModel : INavigatable
    {
        return AddCurrentContentAsync(obj => setup.Invoke((TViewModel)obj))
            .IfSuccessAsync(
                () =>
                {
                    if (Content.Content is IRefresh refresh && Content.Content is TViewModel vm)
                    {
                        return this.InvokeUIBackgroundAsync(() => setup.Invoke(vm))
                            .IfSuccessAsync(() => refresh.RefreshAsync(cancellationToken));
                    }

                    if (Content.Content is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }

                    var viewModel = Resolver.Get<TViewModel>();

                    return this.InvokeUIBackgroundAsync(
                        () =>
                        {
                            setup.Invoke(viewModel);
                            Content.Content = viewModel;
                        }
                    );
                }
            );
    }

    public ConfiguredValueTaskAwaitable<Result> NavigateToAsync<TViewModel>(CancellationToken cancellationToken)
        where TViewModel : INavigatable
    {
        var viewModel = Resolver.Get<TViewModel>();

        return AddCurrentContentAsync(ActionHelper<object>.Empty)
            .IfSuccessAsync(() => this.InvokeUIBackgroundAsync(() => Content.Content = viewModel));
    }

    public ConfiguredValueTaskAwaitable<Result<INavigatable?>> NavigateBackAsync(CancellationToken cancellationToken)
    {
        var item = list.Pop();

        if (item is null)
        {
            return Result<INavigatable?>.DefaultAwaitableFalse;
        }

        return DialogViewer.CloseLastDialogAsync(cancellationToken)
            .IfSuccessAsync(
                value =>
                {
                    if (value)
                    {
                        return new EmptyNavigatable().CastObject<INavigatable?>()
                            .ToValueTaskResult()
                            .ConfigureAwait(false);
                    }

                    if (MainSplitViewModel.IsPaneOpen)
                    {
                        MainSplitViewModel.IsPaneOpen = false;

                        return new EmptyNavigatable().CastObject<INavigatable?>()
                            .ToValueTaskResult()
                            .ConfigureAwait(false);
                    }

                    return this.InvokeUIBackgroundAsync(
                            async () =>
                            {
                                item.Setup.Invoke(item.Navigatable);
                                Content.Content = item.Navigatable;

                                if (item.Navigatable is IRefresh refresh)
                                {
                                    await refresh.RefreshAsync(cancellationToken);
                                }
                            }
                        )
                        .ConfigureAwait(false)
                        .IfSuccessAsync(() => item.Navigatable.ToResult().ToValueTaskResult().ConfigureAwait(false));
                }
            );
    }

    public ConfiguredValueTaskAwaitable<Result> NavigateToAsync<TViewModel>(
        TViewModel parameter,
        CancellationToken cancellationToken
    )
        where TViewModel : INavigatable
    {
        return AddCurrentContentAsync(ActionHelper<object>.Empty)
            .IfSuccessAsync(() => this.InvokeUIBackgroundAsync(() => Content.Content = parameter));
    }
}
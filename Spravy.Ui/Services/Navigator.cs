namespace Spravy.Ui.Services;

public class Navigator : INavigator
{
    private readonly QueryList<NavigatorItem> list = new(5);
    private readonly IDialogViewer dialogViewer;
    private readonly MainSplitViewModel mainSplitViewModel;

    private Action<object> lastSetup = ActionHelper<object>.Empty;

    public Navigator(IDialogViewer dialogViewer, MainSplitViewModel mainSplitViewModel)
    {
        this.dialogViewer = dialogViewer;
        this.mainSplitViewModel = mainSplitViewModel;
    }

    public ConfiguredValueTaskAwaitable<Result<INavigatable>> NavigateBackAsync(
        CancellationToken ct
    )
    {
        return list.Pop()
            .IfSuccessAsync(
                item =>
                    this.InvokeUiBackgroundAsync(() =>
                        {
                            mainSplitViewModel.IsPaneOpen = false;

                            return Result.Success;
                        })
                        .IfSuccessAsync(() => dialogViewer.CloseLastDialogAsync(ct), ct)
                        .IfSuccessAsync(
                            value =>
                            {
                                if (value)
                                {
                                    return new EmptyNavigatable()
                                        .CastObject<INavigatable>(nameof(EmptyNavigatable))
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false);
                                }

                                if (mainSplitViewModel.IsPaneOpen)
                                {
                                    mainSplitViewModel.IsPaneOpen = false;

                                    return new EmptyNavigatable()
                                        .CastObject<INavigatable>(nameof(EmptyNavigatable))
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false);
                                }

                                return this.InvokeUiBackgroundAsync(() =>
                                    {
                                        item.Setup.Invoke(item.Navigatable);
                                        mainSplitViewModel.Content = item.Navigatable;

                                        return Result.Success;
                                    })
                                    .IfSuccessAsync(
                                        () =>
                                        {
                                            if (item.Navigatable is IRefresh refresh)
                                            {
                                                return refresh.RefreshAsync(ct);
                                            }

                                            return Result.AwaitableSuccess;
                                        },
                                        ct
                                    )
                                    .IfSuccessAsync(
                                        () =>
                                            item
                                                .Navigatable.ToResult()
                                                .ToValueTaskResult()
                                                .ConfigureAwait(false),
                                        ct
                                    );
                            },
                            ct
                        ),
                ct
            );
    }

    public Cvtar NavigateToAsync(INavigatable parameter, CancellationToken ct)
    {
        return AddCurrentContentAsync(parameter, ct)
            .IfSuccessAsync(
                () =>
                {
                    if (mainSplitViewModel.Content is not null)
                    {
                        if (parameter.ViewId == mainSplitViewModel.Content.ViewId)
                        {
                            return mainSplitViewModel.Content.RefreshAsync(ct);
                        }
                    }

                    return parameter
                        .LoadStateAsync(ct)
                        .IfSuccessAsync(
                            () =>
                                this.InvokeUiBackgroundAsync(() =>
                                {
                                    mainSplitViewModel.Content = parameter;

                                    return Result.Success;
                                }),
                            ct
                        );
                },
                ct
            )
            .IfSuccessAsync(() => parameter.LoadStateAsync(ct), ct);
    }

    private Cvtar AddCurrentContentAsync(INavigatable parameter, CancellationToken ct)
    {
        if (mainSplitViewModel.Content is null)
        {
            return Result.AwaitableSuccess;
        }

        if (mainSplitViewModel.Content.ViewId == parameter.ViewId)
        {
            return Result.AwaitableSuccess;
        }

        mainSplitViewModel.Content.Stop();

        if (!mainSplitViewModel.Content.IsPooled)
        {
            return Result.AwaitableSuccess;
        }

        return mainSplitViewModel
            .Content.SaveStateAsync(ct)
            .IfSuccessAsync(() => list.Add(new(mainSplitViewModel.Content, lastSetup)), ct)
            .IfSuccessAsync(
                () =>
                    this.InvokeUiBackgroundAsync(() =>
                    {
                        mainSplitViewModel.IsPaneOpen = false;

                        return Result.Success;
                    }),
                ct
            );
    }
}

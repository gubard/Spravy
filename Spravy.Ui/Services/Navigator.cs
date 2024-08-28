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
        return this.InvokeUiBackgroundAsync(() =>
            {
                mainSplitViewModel.IsPaneOpen = false;

                return Result.Success;
            })
            .IfSuccessAsync(
                () =>
                {
                    if (mainSplitViewModel.Content.ViewId == parameter.ViewId)
                    {
                        return mainSplitViewModel.Content.RefreshAsync(ct);
                    }

                    return Result
                        .AwaitableSuccess.IfSuccessAsync(
                            () =>
                            {
                                if (!mainSplitViewModel.Content.IsPooled)
                                {
                                    return list.Add(new(mainSplitViewModel.Content, lastSetup))
                                        .GetAwaitable();
                                }

                                return Result.AwaitableSuccess;
                            },
                            ct
                        )
                        .IfSuccessAsync(
                            () =>
                                parameter
                                    .LoadStateAsync(ct)
                                    .IfSuccessAsync(
                                        () => mainSplitViewModel.Content.SaveStateAsync(ct),
                                        ct
                                    )
                                    .IfSuccessAsync(
                                        () =>
                                            this.InvokeUiBackgroundAsync(() =>
                                            {
                                                mainSplitViewModel.Content = parameter;

                                                return Result.Success;
                                            }),
                                        ct
                                    ),
                            ct
                        );
                },
                ct
            );
    }
}

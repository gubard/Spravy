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
                                        .CastObject<INavigatable>()
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false);
                                }

                                if (mainSplitViewModel.IsPaneOpen)
                                {
                                    mainSplitViewModel.IsPaneOpen = false;

                                    return new EmptyNavigatable()
                                        .CastObject<INavigatable>()
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
        return AddCurrentContentAsync(ActionHelper<object>.Empty, ct)
            .IfSuccessAsync(
                () =>
                    this.InvokeUiBackgroundAsync(() =>
                    {
                        mainSplitViewModel.Content = parameter;

                        return Result.Success;
                    }),
                ct
            );
    }

    private Cvtar AddCurrentContentAsync(Action<object> setup, CancellationToken ct)
    {
        if (mainSplitViewModel.Content is null)
        {
            return Result.AwaitableSuccess;
        }

        var content = (INavigatable)mainSplitViewModel.Content;
        content.Stop();

        if (!content.IsPooled)
        {
            return Result.AwaitableSuccess;
        }

        return content
            .SaveStateAsync(ct)
            .IfSuccessAsync(() => list.Add(new(content, lastSetup)), ct)
            .IfSuccessAsync(
                () =>
                {
                    lastSetup = setup;

                    return Result.AwaitableSuccess;
                },
                ct
            )
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

namespace Spravy.Ui.Services;

public class Navigator : INavigator
{
    private readonly QueryList<NavigatorItem> list = new(5);
    private readonly IDialogViewer dialogViewer;
    private readonly MainSplitViewModel mainSplitViewModel;
    private readonly IUiApplicationService uiApplicationService;

    private Action<object> lastSetup = ActionHelper<object>.Empty;

    public Navigator(
        IDialogViewer dialogViewer,
        IRootViewFactory rootViewFactory,
        IUiApplicationService uiApplicationService
    )
    {
        this.dialogViewer = dialogViewer;
        this.uiApplicationService = uiApplicationService;
        mainSplitViewModel = rootViewFactory.CreateMainSplitViewModel();
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
                                        () => uiApplicationService.RefreshCurrentViewAsync(ct),
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
                        return uiApplicationService.RefreshCurrentViewAsync(ct);
                    }

                    return Result
                        .AwaitableSuccess.IfSuccessAsync(
                            () =>
                            {
                                if (mainSplitViewModel.Content.IsPooled)
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
                                    .IfSuccessAsync(() => mainSplitViewModel.Content.Stop(), ct)
                                    .IfSuccessAsync(
                                        () => mainSplitViewModel.Content.SaveStateAsync(ct),
                                        ct
                                    )
                                    .IfSuccessAsync(
                                        () =>
                                            this.InvokeUiAsync(() =>
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

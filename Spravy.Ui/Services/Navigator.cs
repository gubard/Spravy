namespace Spravy.Ui.Services;

public class Navigator : INavigator
{
    private readonly IDialogViewer dialogViewer;
    private readonly QueryList<INavigatable> list = new(5);
    private readonly MainSplitViewModel mainSplitViewModel;
    private readonly IUiApplicationService uiApplicationService;

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

    public ConfiguredValueTaskAwaitable<Result<INavigatable>> NavigateBackAsync(CancellationToken ct)
    {
        return list.Pop()
           .IfSuccessAsync(
                item => this.InvokeUiBackgroundAsync(
                        () =>
                        {
                            mainSplitViewModel.IsPaneOpen = false;

                            return Result.Success;
                        }
                    )
                   .IfSuccessAsync(() => dialogViewer.CloseLastDialogAsync(ct), ct)
                   .IfSuccessAsync(
                        value =>
                        {
                            if (value)
                            {
                                return new EmptyNavigatable().CastObject<INavigatable>(nameof(EmptyNavigatable))
                                   .ToValueTaskResult()
                                   .ConfigureAwait(false);
                            }

                            if (mainSplitViewModel.IsPaneOpen)
                            {
                                mainSplitViewModel.IsPaneOpen = false;

                                return new EmptyNavigatable().CastObject<INavigatable>(nameof(EmptyNavigatable))
                                   .ToValueTaskResult()
                                   .ConfigureAwait(false);
                            }

                            return this.InvokeUiBackgroundAsync(
                                    () =>
                                    {
                                        mainSplitViewModel.Content = item;

                                        return Result.Success;
                                    }
                                )
                               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct)
                               .IfSuccessAsync(() => item.ToResult().ToValueTaskResult().ConfigureAwait(false), ct);
                        },
                        ct
                    ),
                ct
            );
    }

    public Cvtar NavigateToAsync(INavigatable parameter, CancellationToken ct)
    {
        return this.InvokeUiBackgroundAsync(
                () =>
                {
                    mainSplitViewModel.IsPaneOpen = false;

                    return Result.Success;
                }
            )
           .IfSuccessAsync(
                () => Result.AwaitableSuccess
                   .IfSuccessAsync(
                        () =>
                        {
                            if (mainSplitViewModel.Content.IsPooled
                             && mainSplitViewModel.Content.ViewId != parameter.ViewId)
                            {
                                return list.Add(mainSplitViewModel.Content).GetAwaitable();
                            }

                            return Result.AwaitableSuccess;
                        },
                        ct
                    )
                   .IfSuccessAsync(
                        () => parameter.LoadStateAsync(ct)
                           .IfSuccessAsync(() => mainSplitViewModel.Content.Stop(), ct)
                           .IfSuccessAsync(() => mainSplitViewModel.Content.SaveStateAsync(ct), ct)
                           .IfSuccessAsync(
                                () => this.InvokeUiAsync(
                                    () =>
                                    {
                                        mainSplitViewModel.Content = parameter;

                                        return Result.Success;
                                    }
                                ),
                                ct
                            ),
                        ct
                    )
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                ct
            );
    }
}
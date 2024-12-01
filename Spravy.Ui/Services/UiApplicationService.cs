namespace Spravy.Ui.Services;

public class UiApplicationService : IUiApplicationService
{
    private readonly AccountNotify accountNotify;
    private readonly MainSplitViewModel mainSplitViewModel;
    private readonly IToDoUiService toDoUiService;

    public UiApplicationService(
        IRootViewFactory rootViewFactory,
        IToDoUiService toDoUiService,
        AccountNotify accountNotify
    )
    {
        this.toDoUiService = toDoUiService;
        this.accountNotify = accountNotify;
        mainSplitViewModel = rootViewFactory.CreateMainSplitViewModel();
    }

    public Result StopCurrentView()
    {
        return mainSplitViewModel.Content.Stop();
    }

    public Cvtar RefreshCurrentViewAsync(CancellationToken ct)
    {
        return mainSplitViewModel.Content
           .RefreshAsync(ct)
           .IfSuccessAsync(
                () => accountNotify.Login.IsNullOrWhiteSpace()
                    ? Result.AwaitableSuccess
                    : toDoUiService.UpdateBookmarkItemsAsync(mainSplitViewModel.Pane, ct),
                ct
            );
    }

    public Result<Type> GetCurrentViewType()
    {
        return mainSplitViewModel.Content
           .IfNotNull(nameof(mainSplitViewModel.Content))
           .IfSuccess(content => content.GetType().ToResult());
    }

    public Result<TView> GetCurrentView<TView>() where TView : notnull
    {
        return mainSplitViewModel.Content
           .IfNotNull(nameof(mainSplitViewModel.Content))
           .IfSuccess(vm => vm.IfIs<TView>());
    }
}
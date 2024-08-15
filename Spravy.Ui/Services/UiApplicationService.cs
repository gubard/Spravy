namespace Spravy.Ui.Services;

public class UiApplicationService : IUiApplicationService
{
    private readonly MainSplitViewModel mainSplitViewModel;

    public UiApplicationService(MainSplitViewModel mainSplitViewModel)
    {
        this.mainSplitViewModel = mainSplitViewModel;
    }

    public Cvtar RefreshCurrentViewAsync(CancellationToken ct)
    {
        if (mainSplitViewModel.Content is not IRefresh refresh)
        {
            return Result.AwaitableSuccess;
        }

        return refresh.RefreshAsync(ct);
    }

    public Result<Type> GetCurrentViewType()
    {
        return mainSplitViewModel
            .Content.IfNotNull(nameof(mainSplitViewModel.Content))
            .IfSuccess(content => content.GetType().ToResult());
    }

    public Result<TView> GetCurrentView<TView>()
        where TView : notnull
    {
        return mainSplitViewModel
            .Content.IfNotNull(nameof(mainSplitViewModel.Content))
            .IfSuccess(vm => vm.IfIs<TView>());
    }
}

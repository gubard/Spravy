namespace Spravy.Ui.Services;

public class UiApplicationService : IUiApplicationService
{
    private readonly MainSplitViewModel mainSplitViewModel;

    public UiApplicationService(IRootViewFactory rootViewFactory)
    {
        mainSplitViewModel = rootViewFactory.CreateMainSplitViewModel();
    }

    public Result StopCurrentView()
    {
        return mainSplitViewModel.Content.Stop();
    }

    public Cvtar RefreshCurrentViewAsync(CancellationToken ct)
    {
        return mainSplitViewModel.Content.RefreshAsync(ct);
    }

    public Result<Type> GetCurrentViewType()
    {
        return mainSplitViewModel.Content.IfNotNull(nameof(mainSplitViewModel.Content)).IfSuccess(content => content.GetType().ToResult());
    }

    public Result<TView> GetCurrentView<TView>() where TView : notnull
    {
        return mainSplitViewModel.Content.IfNotNull(nameof(mainSplitViewModel.Content)).IfSuccess(vm => vm.IfIs<TView>());
    }
}
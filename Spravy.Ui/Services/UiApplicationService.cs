namespace Spravy.Ui.Services;

public class UiApplicationService : IUiApplicationService
{
    private readonly MainSplitViewModel mainSplitViewModel;
    
    public UiApplicationService(MainSplitViewModel mainSplitViewModel)
    {
        this.mainSplitViewModel = mainSplitViewModel;
    }
    
    public ConfiguredValueTaskAwaitable<Result> RefreshCurrentViewAsync(CancellationToken cancellationToken)
    {
        if (mainSplitViewModel.Content is not IRefresh refresh)
        {
            return Result.AwaitableSuccess;
        }
        
        return refresh.RefreshAsync(cancellationToken);
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
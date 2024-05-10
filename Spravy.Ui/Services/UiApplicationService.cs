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
            return Result.AwaitableFalse;
        }
        
        return refresh.RefreshAsync(cancellationToken);
    }
}
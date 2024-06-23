namespace Spravy.Ui.Services;

public class Navigator : INavigator
{
    private readonly QueryList<NavigatorItem> list = new(5);
    private readonly IServiceFactory serviceFactory;
    private readonly IDialogViewer dialogViewer;
    private readonly MainSplitViewModel mainSplitViewModel;
    
    private Action<object> lastSetup = ActionHelper<object>.Empty;
    
    public Navigator(
        IDialogViewer dialogViewer,
        IServiceFactory serviceFactory,
        MainSplitViewModel mainSplitViewModel
    )
    {
        this.dialogViewer = dialogViewer;
        this.serviceFactory = serviceFactory;
        this.mainSplitViewModel = mainSplitViewModel;
    }
    
    public ConfiguredValueTaskAwaitable<Result> NavigateToAsync<TViewModel>(
        Action<TViewModel> setup,
        CancellationToken cancellationToken
    ) where TViewModel : INavigatable
    {
        return AddCurrentContentAsync(obj => setup.Invoke((TViewModel)obj), cancellationToken)
           .IfSuccessAsync(() =>
            {
                if (mainSplitViewModel.Content is IRefresh refresh && mainSplitViewModel.Content is TViewModel vm)
                {
                    return this.InvokeUiBackgroundAsync(() =>
                        {
                            setup.Invoke(vm);
                            
                            return Result.Success;
                        })
                       .IfSuccessAsync(() => refresh.RefreshAsync(cancellationToken), cancellationToken);
                }
                
                if (mainSplitViewModel.Content is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                
                var viewModel = serviceFactory.CreateService<TViewModel>();
                
                return this.InvokeUiBackgroundAsync(() =>
                {
                    setup.Invoke(viewModel);
                    mainSplitViewModel.Content = viewModel;
                    
                    return Result.Success;
                });
            }, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> NavigateToAsync<TViewModel>(CancellationToken cancellationToken)
        where TViewModel : INavigatable
    {
        var viewModel = serviceFactory.CreateService<TViewModel>();
        
        return AddCurrentContentAsync(ActionHelper<object>.Empty, cancellationToken)
           .IfSuccessAsync(() => this.InvokeUiBackgroundAsync(() =>
            {
                mainSplitViewModel.Content = viewModel;
                
                return Result.Success;
            }), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<INavigatable>> NavigateBackAsync(CancellationToken cancellationToken)
    {
        var item = list.Pop();
        
        if (item is null)
        {
            return new Result<INavigatable>(new NavigatorCacheEmptyError()).ToValueTaskResult().ConfigureAwait(false);
        }
        
        return dialogViewer.CloseLastDialogAsync(cancellationToken)
           .IfSuccessAsync(value =>
            {
                if (value)
                {
                    return new EmptyNavigatable().CastObject<INavigatable>().ToValueTaskResult().ConfigureAwait(false);
                }
                
                if (mainSplitViewModel.IsPaneOpen)
                {
                    mainSplitViewModel.IsPaneOpen = false;
                    
                    return new EmptyNavigatable().CastObject<INavigatable>().ToValueTaskResult().ConfigureAwait(false);
                }
                
                return this.InvokeUiBackgroundAsync(() =>
                    {
                        item.Setup.Invoke(item.Navigatable);
                        mainSplitViewModel.Content = item.Navigatable;
                        
                        return Result.Success;
                    })
                   .IfSuccessAsync(() =>
                    {
                        if (item.Navigatable is IRefresh refresh)
                        {
                            return refresh.RefreshAsync(cancellationToken);
                        }
                        
                        return Result.AwaitableSuccess;
                    }, cancellationToken)
                   .IfSuccessAsync(() => item.Navigatable.ToResult().ToValueTaskResult().ConfigureAwait(false),
                        cancellationToken);
            }, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> NavigateToAsync<TViewModel>(
        TViewModel parameter,
        CancellationToken cancellationToken
    ) where TViewModel : INavigatable
    {
        return AddCurrentContentAsync(ActionHelper<object>.Empty, cancellationToken)
           .IfSuccessAsync(() => this.InvokeUiBackgroundAsync(() =>
            {
                mainSplitViewModel.Content = parameter;
                
                return Result.Success;
            }), cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> AddCurrentContentAsync(
        Action<object> setup,
        CancellationToken cancellationToken
    )
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
        
        return content.SaveStateAsync(cancellationToken)
           .IfSuccessAsync(() =>
            {
                list.Add(new(content, lastSetup));
                lastSetup = setup;
                
                return Result.AwaitableSuccess;
            }, cancellationToken);
    }
}
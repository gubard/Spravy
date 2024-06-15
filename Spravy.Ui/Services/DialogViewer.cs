namespace Spravy.Ui.Services;

public class DialogViewer : IDialogViewer
{
    public const string ContentDialogHostIdentifier = "ContentDialogHost";
    public const string ErrorDialogHostIdentifier = "ErrorDialogHost";
    public const string InputDialogHostIdentifier = "InputDialogHost";
    public const string ProgressDialogHostIdentifier = "ProgressDialogHost";
    private readonly IServiceFactory serviceFactory;
    
    public DialogViewer(IServiceFactory serviceFactory)
    {
        this.serviceFactory = serviceFactory;
    }
    
    public ConfiguredValueTaskAwaitable<Result> ShowContentDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        
        return this.InvokeUiBackgroundAsync(() =>
            {
                 setupView.Invoke(content);
                 
                 return Result.Success;
            })
           .IfSuccessAsync(() => ShowView(content, ContentDialogHostIdentifier),
                cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> ShowProgressDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        
        return this.InvokeUiBackgroundAsync(() =>
            {
                 setupView.Invoke(content);
                 
                 return Result.Success;
            })
           .IfSuccessAsync(() => ShowView(content, ProgressDialogHostIdentifier),
                cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> ShowErrorDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        
        return this.InvokeUiBackgroundAsync(() =>
            {
                 setupView.Invoke(content);
                 
                 return Result.Success;
            })
           .IfSuccessAsync(() => ShowView(content, ErrorDialogHostIdentifier), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> ShowInfoErrorDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        
        return this.InvokeUiBackgroundAsync(() =>
            {
                 setupView.Invoke(content);
                 
                 return Result.Success;
            })
           .IfSuccessAsync(() =>
            {
                var infoViewModel = serviceFactory.CreateService<InfoViewModel>();
                infoViewModel.Content = content;
                infoViewModel.OkTask = view => okTask.Invoke((TView)view);
                
                return ShowView(infoViewModel, ErrorDialogHostIdentifier);
            }, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> ShowInfoInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        
        return this.InvokeUiBackgroundAsync(() =>
            {
                 setupView.Invoke(content);
                 
                 return Result.Success;
            })
           .IfSuccessAsync(() =>
            {
                var infoViewModel = serviceFactory.CreateService<InfoViewModel>();
                infoViewModel.Content = content;
                infoViewModel.OkTask = view => okTask.Invoke((TView)view);
                
                return ShowView(infoViewModel, InputDialogHostIdentifier);
            }, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> ShowInfoContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        
        return this.InvokeUiBackgroundAsync(() =>
            {
                 setupView.Invoke(content);
                 
                 return Result.Success;
            })
           .IfSuccessAsync(() =>
            {
                var infoViewModel = serviceFactory.CreateService<InfoViewModel>();
                infoViewModel.Content = content;
                infoViewModel.OkTask = view => okTask.Invoke((TView)view);
                
                return ShowView(infoViewModel, ContentDialogHostIdentifier);
            }, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> ShowInputDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        
        if (content is null)
        {
            return new Result(new NotFoundTypeError(typeof(TView))).ToValueTaskResult().ConfigureAwait(false);
        }
        
        return this.InvokeUiBackgroundAsync(() =>
            {
                 setupView.Invoke(content);
                 
                 return Result.Success;
            })
           .IfSuccessAsync(() => ShowView(content, InputDialogHostIdentifier), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> CloseProgressDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ProgressDialogHostIdentifier, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<bool>> CloseLastDialogAsync(CancellationToken cancellationToken)
    {
        if (DialogHost.IsDialogOpen(ProgressDialogHostIdentifier))
        {
            return SafeClose(ProgressDialogHostIdentifier, cancellationToken)
               .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false), cancellationToken);
        }
        
        if (DialogHost.IsDialogOpen(ErrorDialogHostIdentifier))
        {
            return SafeClose(ErrorDialogHostIdentifier, cancellationToken)
               .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false), cancellationToken);
        }
        
        if (DialogHost.IsDialogOpen(InputDialogHostIdentifier))
        {
            return SafeClose(InputDialogHostIdentifier, cancellationToken)
               .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false), cancellationToken);
        }
        
        if (DialogHost.IsDialogOpen(ContentDialogHostIdentifier))
        {
            return SafeClose(ContentDialogHostIdentifier, cancellationToken)
               .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false), cancellationToken);
        }
        
        return false.ToResult().ToValueTaskResult().ConfigureAwait(false);
    }
    
    public ConfiguredValueTaskAwaitable<Result> ShowConfirmContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        
        return this.InvokeUiBackgroundAsync(() =>
            {
                 setupView.Invoke(content);
                 
                 return Result.Success;
            })
           .IfSuccessAsync(() =>
            {
                var confirmViewModel = serviceFactory.CreateService<ConfirmViewModel>();
                confirmViewModel.Content = content;
                confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
                confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);
                
                return ShowView(confirmViewModel, ContentDialogHostIdentifier);
            }, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> ShowConfirmInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        
        return this.InvokeUiBackgroundAsync(() =>
            {
                 setupView.Invoke(content);
                 
                 return Result.Success;
            })
           .IfSuccessAsync(() =>
            {
                var confirmViewModel = serviceFactory.CreateService<ConfirmViewModel>();
                confirmViewModel.Content = content;
                confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
                confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);
                
                return ShowView(confirmViewModel, InputDialogHostIdentifier);
            }, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> CloseContentDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ContentDialogHostIdentifier, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> CloseErrorDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ErrorDialogHostIdentifier, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> CloseInputDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(InputDialogHostIdentifier, cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> ShowView(object content, string identifier)
    {
        if (DialogHost.IsDialogOpen(identifier))
        {
            return Result.AwaitableSuccess;
        }
        
        return this.InvokeUiBackgroundAsync(() =>
        {
             DialogHost.Show(content, identifier);
             
             return Result.Success;
        });
    }
    
    private ConfiguredValueTaskAwaitable<Result> SafeClose(string identifier, CancellationToken cancellationToken)
    {
        if (!DialogHost.IsDialogOpen(identifier))
        {
            return Result.AwaitableSuccess;
        }
        
        return this.InvokeUiBackgroundAsync(() =>
                DialogHost.GetDialogSession(identifier)
                   .IfNotNull("DialogSession")
                   .IfSuccess(session => session.Content.IfNotNull(nameof(session.Content))))
           .IfSuccessAsync(content =>
            {
                if (content is ISaveState saveState)
                {
                    return saveState.SaveStateAsync(cancellationToken);
                }
                
                return Result.AwaitableSuccess;
            }, cancellationToken)
           .IfSuccessAsync(() => this.InvokeUiBackgroundAsync(() =>
            {
                 DialogHost.Close(identifier);
                 
                 return Result.Success;
            }), cancellationToken);
    }
}
namespace Spravy.Ui.ViewModels;

public class ConfirmViewModel : ViewModelBase, ISaveState
{
    public ConfirmViewModel(ITaskProgressService taskProgressService, IErrorHandler errorHandler)
    {
        ConfirmCommand = SpravyCommand.Create(
            cancellationToken => taskProgressService.RunProgressAsync(
                () => ConfirmTask.IfNotNull(nameof(ConfirmTask))
                   .IfSuccessAsync(
                        confirm => Content.IfNotNull(nameof(Content)).IfSuccessAsync(confirm, cancellationToken),
                        cancellationToken), cancellationToken), errorHandler);
        
        CancelCommand = CreateCommandFromTask(async () => await CancelAsync());
    }
    
    public Func<object, ConfiguredValueTaskAwaitable<Result>>? ConfirmTask { get; set; }
    public Func<object, ConfiguredValueTaskAwaitable<Result>>? CancelTask { get; set; }
    public ICommand CancelCommand { get; }
    public SpravyCommand ConfirmCommand { get; }
    
    [Reactive]
    public object? Content { get; set; }
    
    public ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        if (Content is ISaveState saveState)
        {
            return saveState.SaveStateAsync(cancellationToken);
        }
        
        return Result.AwaitableSuccess;
    }
    
    private async ValueTask<Result> CancelAsync()
    {
        var con = Content.ThrowIfNull();
        
        return await CancelTask.ThrowIfNull().Invoke(con);
    }
    
    private async ValueTask<Result> ConfirmAsync()
    {
        var con = Content.ThrowIfNull();
        
        return await ConfirmTask.ThrowIfNull().Invoke(con);
    }
}
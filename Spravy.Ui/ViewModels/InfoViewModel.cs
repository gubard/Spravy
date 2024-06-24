namespace Spravy.Ui.ViewModels;

public class InfoViewModel : ViewModelBase
{
    public InfoViewModel(IErrorHandler errorHandler, ITaskProgressService taskProgressService)
    {
        OkCommand = SpravyCommand.Create(OkAsync, errorHandler, taskProgressService);
    }
    
    [Reactive]
    public object? Content { get; set; }
    
    public Func<object, ConfiguredValueTaskAwaitable<Result>>? OkTask { get; set; }
    public SpravyCommand OkCommand { get; }
    
    private ConfiguredValueTaskAwaitable<Result> OkAsync(CancellationToken ct)
    {
        var con = Content.ThrowIfNull();
        
        return OkTask.ThrowIfNull().Invoke(con);
    }
}
namespace Spravy.Ui.Features.ErrorHandling.ViewModels;

public class ExceptionViewModel : ViewModelBase
{
    public ExceptionViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    [Reactive]
    public Exception? Exception { get; set; }

    [Reactive]
    public string Message { get; set; } = string.Empty;

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        this.WhenAnyValue(x => x.Exception).Subscribe(x => Message = x?.Message ?? string.Empty);

        return Result.AwaitableFalse;
    }
}
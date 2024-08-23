namespace Spravy.Ui.ViewModels;

public class ConfirmViewModel : ViewModelBase
{
    private readonly Func<object, ConfiguredValueTaskAwaitable<Result>> confirmTask;
    private readonly Func<object, ConfiguredValueTaskAwaitable<Result>> cancelTask;

    public ConfirmViewModel(
        object content,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        Func<object, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<object, ConfiguredValueTaskAwaitable<Result>> cancelTask
    )
    {
        this.confirmTask = confirmTask;
        this.cancelTask = cancelTask;
        Content = content;
        ConfirmCommand = SpravyCommand.Create(ConfirmAsync, errorHandler, taskProgressService);
        CancelCommand = SpravyCommand.Create(CancelAsync, errorHandler, taskProgressService);
    }

    public SpravyCommand CancelCommand { get; }
    public SpravyCommand ConfirmCommand { get; }
    public object Content { get; }

    private Cvtar CancelAsync(CancellationToken ct)
    {
        return cancelTask.Invoke(Content);
    }

    private Cvtar ConfirmAsync(CancellationToken ct)
    {
        return confirmTask.Invoke(Content);
    }
}

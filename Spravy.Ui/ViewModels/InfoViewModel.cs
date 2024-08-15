namespace Spravy.Ui.ViewModels;

public partial class InfoViewModel : ViewModelBase
{
    private readonly Func<object, ConfiguredValueTaskAwaitable<Result>> okTask;

    public InfoViewModel(
        object content,
        Func<object, ConfiguredValueTaskAwaitable<Result>> okTask,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        Content = content;
        this.okTask = okTask;
        OkCommand = SpravyCommand.Create(OkAsync, errorHandler, taskProgressService);
    }

    public SpravyCommand OkCommand { get; }
    public object Content { get; }

    private Cvtar OkAsync(CancellationToken ct)
    {
        return okTask.Invoke(Content);
    }
}

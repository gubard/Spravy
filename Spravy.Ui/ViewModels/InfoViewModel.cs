namespace Spravy.Ui.ViewModels;

public partial class InfoViewModel : ViewModelBase
{
    [ObservableProperty]
    private object? content;

    public InfoViewModel(IErrorHandler errorHandler, ITaskProgressService taskProgressService)
    {
        OkCommand = SpravyCommand.Create(OkAsync, errorHandler, taskProgressService);
    }

    public Func<object, ConfiguredValueTaskAwaitable<Result>>? OkTask { get; set; }
    public SpravyCommand OkCommand { get; }

    private ConfiguredValueTaskAwaitable<Result> OkAsync(CancellationToken ct)
    {
        var con = Content.ThrowIfNull();

        return OkTask.ThrowIfNull().Invoke(con);
    }
}

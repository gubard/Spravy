namespace Spravy.Ui.ViewModels;

public class ConfirmViewModel : ViewModelBase, ISaveState
{
    public ConfirmViewModel(IErrorHandler errorHandler, ITaskProgressService taskProgressService)
    {
        ConfirmCommand = SpravyCommand.Create(
            ct =>
                ConfirmTask
                    .IfNotNull(nameof(ConfirmTask))
                    .IfSuccessAsync(
                        confirm => Content.IfNotNull(nameof(Content)).IfSuccessAsync(confirm, ct),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        CancelCommand = SpravyCommand.Create(CancelAsync, errorHandler, taskProgressService);
    }

    public Func<object, ConfiguredValueTaskAwaitable<Result>>? ConfirmTask { get; set; }
    public Func<object, ConfiguredValueTaskAwaitable<Result>>? CancelTask { get; set; }
    public SpravyCommand CancelCommand { get; }
    public SpravyCommand ConfirmCommand { get; }

    [Reactive]
    public object? Content { get; set; }

    public ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        if (Content is ISaveState saveState)
        {
            return saveState.SaveStateAsync(ct);
        }

        return Result.AwaitableSuccess;
    }

    private ConfiguredValueTaskAwaitable<Result> CancelAsync(CancellationToken ct)
    {
        var con = Content.ThrowIfNull();

        return CancelTask.ThrowIfNull().Invoke(con);
    }

    private async ValueTask<Result> ConfirmAsync()
    {
        var con = Content.ThrowIfNull();

        return await ConfirmTask.ThrowIfNull().Invoke(con);
    }
}

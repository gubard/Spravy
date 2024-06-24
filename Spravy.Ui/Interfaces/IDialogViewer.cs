namespace Spravy.Ui.Interfaces;

public interface IDialogViewer
{
    ConfiguredValueTaskAwaitable<Result> ShowContentDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken ct
    ) where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowProgressDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken ct
    ) where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowErrorDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken ct
    ) where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowInfoErrorDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken ct
    ) where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowInfoInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken ct
    ) where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowInfoContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken ct
    ) where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowInputDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken ct
    ) where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> CloseContentDialogAsync(CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result> CloseErrorDialogAsync(CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result> CloseInputDialogAsync(CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result> CloseProgressDialogAsync(CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result<bool>> CloseLastDialogAsync(CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result> ShowConfirmContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken ct
    ) where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowConfirmInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken ct
    ) where TView : ViewModelBase;
}
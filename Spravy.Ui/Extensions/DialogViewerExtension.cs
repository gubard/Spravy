namespace Spravy.Ui.Extensions;

public static class DialogViewerExtension
{
    public static ConfiguredValueTaskAwaitable<Result> ShowConfirmContentDialogAsync<TView>(
        this IDialogViewer dialogViewer,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        return dialogViewer.ShowConfirmContentDialogAsync(
            confirmTask,
            _ => dialogViewer.CloseContentDialogAsync(ct),
            ActionHelper<TView>.Empty,
            Result<TView>.EmptyFunc,
            ct
        );
    }

    public static ConfiguredValueTaskAwaitable<Result> ShowConfirmContentDialogAsync<TView>(
        this IDialogViewer dialogViewer,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<TView> setup,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        return dialogViewer.ShowConfirmContentDialogAsync(
            confirmTask,
            _ => dialogViewer.CloseContentDialogAsync(ct),
            setup,
            Result<TView>.EmptyFunc,
            ct
        );
    }

    public static ConfiguredValueTaskAwaitable<Result> ShowSingleStringConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<string, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<TextViewModel> setup,
        Func<TextViewModel, ConfiguredValueTaskAwaitable<Result>> initialized,
        CancellationToken ct
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.Text.ThrowIfNull()),
            _ => dialogViewer.CloseInputDialogAsync(ct),
            setup,
            initialized,
            ct
        );
    }

    public static ConfiguredValueTaskAwaitable<Result> ShowSingleStringConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<string, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        CancellationToken ct
    )
    {
        return dialogViewer.ShowSingleStringConfirmDialogAsync(
            confirmTask,
            ActionHelper<TextViewModel>.Empty,
            Result<TextViewModel>.EmptyFunc,
            ct
        );
    }

    public static ConfiguredValueTaskAwaitable<Result> ShowToDoItemSelectorConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<ToDoItemEntityNotify, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<ToDoItemSelectorViewModel> setup,
        Func<ToDoItemSelectorViewModel, ConfiguredValueTaskAwaitable<Result>> initialized,
        CancellationToken ct
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.SelectedItem.ThrowIfNull()),
            _ => dialogViewer.CloseInputDialogAsync(ct),
            setup,
            initialized,
            ct
        );
    }

    public static ConfiguredValueTaskAwaitable<Result> ShowToDoItemSelectorConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<ToDoItemEntityNotify, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<ToDoItemSelectorViewModel> setup,
        CancellationToken ct
    )
    {
        return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
            confirmTask,
            setup,
            Result<ToDoItemSelectorViewModel>.EmptyFunc,
            ct
        );
    }
}

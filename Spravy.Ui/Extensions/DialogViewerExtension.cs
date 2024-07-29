namespace Spravy.Ui.Extensions;

public static class DialogViewerExtension
{
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
}

using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Views;

namespace Spravy.Ui.Extensions;

public static class DialogViewerExtension
{
    public static Task ShowStringConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<string, Task> confirmTask,
        Action<TextBox>? setup = null
    )
    {
        return dialogViewer.ShowConfirmDialogAsync(
            _ =>
            {
                dialogViewer.CloseDialog();

                return Task.CompletedTask;
            },
            view => confirmTask.Invoke(view.Text.ThrowIfNull()),
            setup
        );
    }

    public static Task ShowDateTimeConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<DateTime, Task> confirmTask,
        Action<Calendar>? setup = null
    )
    {
        return dialogViewer.ShowConfirmDialogAsync(
            _ =>
            {
                dialogViewer.CloseDialog();

                return Task.CompletedTask;
            },
            view => confirmTask.Invoke(view.SelectedDate.ThrowIfNullStruct()),
            setup
        );
    }

    public static Task ShowCompleteToDoItemDialogAsync(
        this IDialogViewer dialogViewer,
        Action<CompleteToDoItemView> setup
    )
    {
        return dialogViewer.ShowDialogAsync(setup);
    }
}
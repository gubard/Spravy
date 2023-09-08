using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;

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
}
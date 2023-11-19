using System;
using System.Threading.Tasks;
using Avalonia.Media;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Extensions;

public static class DialogViewerExtension
{
    public static Task ShowSingleStringConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<string, Task> confirmTask,
        Action<TextViewModel>? setup = null
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.Text.ThrowIfNull()),
            _ => dialogViewer.CloseInputDialogAsync(),
            setup
        );
    }

    public static Task ShowMultiStringConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<string, Task> confirmTask,
        Action<TextViewModel>? setup = null
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.Text.ThrowIfNull()),
            _ => dialogViewer.CloseInputDialogAsync(),
            (TextViewModel textBox) =>
            {
                textBox.AcceptsReturn = true;
                textBox.TextWrapping = TextWrapping.Wrap;
                setup?.Invoke(textBox);
            }
        );
    }

    public static Task ShowDateConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<DateTime, Task> confirmTask,
        Action<CalendarViewModel>? setup = null
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.SelectedDate.ThrowIfNullStruct()),
            _ => dialogViewer.CloseInputDialogAsync(),
            setup
        );
    }

    public static Task ShowDateTimeConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<DateTime, Task> confirmTask,
        Action<DateTimeViewModel>? setupCalendar = null
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync<DateTimeViewModel>(
            viewModel => confirmTask.Invoke(
                viewModel.SelectedDate.ThrowIfNullStruct().Add(viewModel.SelectedTime.ThrowIfNullStruct())
            ),
            _ => dialogViewer.CloseInputDialogAsync(),
            viewModel => setupCalendar?.Invoke(viewModel)
        );
    }

    public static Task ShowToDoItemSelectorConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<ToDoSelectorItemNotify, Task> confirmTask,
        Action<ToDoItemSelectorViewModel> setup
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.SelectedItem.ThrowIfNull()),
            _ => dialogViewer.CloseInputDialogAsync(),
            setup
        );
    }
}
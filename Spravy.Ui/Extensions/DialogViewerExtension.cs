using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Material.Styles.Controls;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.Extensions;

public static class DialogViewerExtension
{
    public static Task ShowSingleStringConfirmDialogAsync(
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

    public static Task ShowMultiStringConfirmDialogAsync(
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
            (TextBox textBox) =>
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

    public static Task ShowDateTimeConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<DateTime, Task> confirmTask,
        Action<Calendar>? setupCalendar = null,
        Action<Clock>? setupClock = null
    )
    {
        var calendar = new Calendar();
        var clock = new Clock();

        return dialogViewer.ShowConfirmDialogAsync<WrapPanel>(
            _ =>
            {
                dialogViewer.CloseDialog();

                return Task.CompletedTask;
            },
            _ => confirmTask.Invoke(
                calendar.SelectedDate.ThrowIfNullStruct().Add(clock.SelectedTime.ThrowIfNullStruct())
            ),
            wrapPanel =>
            {
                wrapPanel.Orientation = Orientation.Horizontal;
                wrapPanel.Children.Add(calendar);
                wrapPanel.Children.Add(clock);
                setupCalendar?.Invoke(calendar);
                setupClock?.Invoke(clock);
            }
        );
    }

    public static Task ShowToDoItemSelectorConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<ToDoSelectorItemNotify, Task> confirmTask,
        Action<ToDoItemSelectorView> setup
    )
    {
        return dialogViewer.ShowConfirmDialogAsync(
            _ =>
            {
                dialogViewer.CloseDialog();

                return Task.CompletedTask;
            },
            view => confirmTask.Invoke(view.ViewModel.ThrowIfNull().SelectedItem.ThrowIfNull()),
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Extensions;

public static class DialogViewerExtension
{
    public static Task ShowNumberUInt16InputDialogAsync(
        this IDialogViewer dialogViewer,
        Func<ushort, Task> confirmTask,
        Action<NumberViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            viewModel => confirmTask.Invoke((ushort)viewModel.Value),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            setup,
            cancellationToken
        );
    }

    public static Task ShowInfoContentDialogAsync<TView>(
        this IDialogViewer dialogViewer,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        return dialogViewer.ShowInfoContentDialogAsync(
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
            setupView,
            cancellationToken
        );
    }

    public static Task ShowSingleStringConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<string, Task> confirmTask,
        Action<TextViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.Text.ThrowIfNull()),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            setup,
            cancellationToken
        );
    }

    public static Task ShowMultiStringConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<string, Task> confirmTask,
        Action<TextViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.Text.ThrowIfNull()),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            (TextViewModel textBox) =>
            {
                textBox.AcceptsReturn = true;
                textBox.TextWrapping = TextWrapping.Wrap;
                setup.Invoke(textBox);
            },
            cancellationToken
        );
    }

    public static Task ShowDayOfWeekSelectorInputDialogAsync(
        this IDialogViewer dialogViewer,
        Func<IEnumerable<DayOfWeek>, Task> confirmTask,
        Action<ToDoItemDayOfWeekSelectorViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.Items.Where(x => x.IsSelected).Select(x => x.DayOfWeek)),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            setup,
            cancellationToken
        );
    }

    public static Task ShowDayOfMonthSelectorInputDialogAsync(
        this IDialogViewer dialogViewer,
        Func<IEnumerable<byte>, Task> confirmTask,
        Action<DayOfMonthSelectorViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.Items.Where(x => x.IsSelected).Select(x => x.Day)),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            setup,
            cancellationToken
        );
    }

    public static Task ShowDayOfYearSelectorInputDialogAsync(
        this IDialogViewer dialogViewer,
        Func<IEnumerable<DayOfYear>, Task> confirmTask,
        Action<DayOfYearSelectorViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(
                view.Items.SelectMany(
                    x => x.Days.Items.Where(y => y.IsSelected).Select(y => new DayOfYear(y.Day, x.Month))
                )
            ),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            setup,
            cancellationToken
        );
    }

    public static Task ShowDateConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<DateTime, Task> confirmTask,
        Action<CalendarViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.SelectedDate.ThrowIfNullStruct()),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            setup,
            cancellationToken
        );
    }

    public static Task ShowItemSelectorDialogAsync<TItem>(
        this IDialogViewer dialogViewer,
        Func<TItem, Task> confirmTask,
        Action<ItemSelectorViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.SelectedItem.ThrowIfNull().ThrowIfIsNotCast<TItem>()),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            setup,
            cancellationToken
        );
    }

    public static Task ShowDateTimeConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<DateTime, Task> confirmTask,
        Action<DateTimeViewModel>? setupCalendar,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync<DateTimeViewModel>(
            viewModel => confirmTask.Invoke(
                viewModel.SelectedDate.ThrowIfNullStruct().Add(viewModel.SelectedTime.ThrowIfNullStruct())
            ),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            viewModel => setupCalendar?.Invoke(viewModel),
            cancellationToken
        );
    }

    public static Task ShowToDoItemSelectorConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<ToDoSelectorItemNotify, Task> confirmTask,
        Action<ToDoItemSelectorViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.SelectedItem.ThrowIfNull()),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            setup,
            cancellationToken
        );
    }
}
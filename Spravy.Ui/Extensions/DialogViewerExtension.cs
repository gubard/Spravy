using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public static ValueTask<Result> ShowNumberUInt16InputDialogAsync(
        this IDialogViewer dialogViewer,
        Func<ushort, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<NumberViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            viewModel => confirmTask.Invoke((ushort)viewModel.Value),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false),
            setup,
            cancellationToken
        );
    }

    public static ValueTask<Result> ShowInfoContentDialogAsync<TView>(
        this IDialogViewer dialogViewer,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        return dialogViewer.ShowInfoContentDialogAsync(
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false),
            setupView,
            cancellationToken
        );
    }

    public static ValueTask<Result> ShowSingleStringConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<string, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<TextViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.Text.ThrowIfNull()),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false),
            setup,
            cancellationToken
        );
    }

    public static ValueTask<Result> ShowMultiStringConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<string, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<TextViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.Text.ThrowIfNull()),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false),
            (TextViewModel textBox) =>
            {
                textBox.AcceptsReturn = true;
                textBox.TextWrapping = TextWrapping.Wrap;
                setup.Invoke(textBox);
            },
            cancellationToken
        );
    }

    public static ValueTask<Result> ShowDayOfWeekSelectorInputDialogAsync(
        this IDialogViewer dialogViewer,
        Func<IEnumerable<DayOfWeek>, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<ToDoItemDayOfWeekSelectorViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.Items.Where(x => x.IsSelected).Select(x => x.DayOfWeek)),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false),
            setup,
            cancellationToken
        );
    }

    public static ValueTask<Result> ShowDayOfMonthSelectorInputDialogAsync(
        this IDialogViewer dialogViewer,
        Func<IEnumerable<byte>, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<ToDoItemDayOfMonthSelectorViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.Items.Where(x => x.IsSelected).Select(x => x.Day)),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false),
            setup,
            cancellationToken
        );
    }

    public static ValueTask<Result> ShowDayOfYearSelectorInputDialogAsync(
        this IDialogViewer dialogViewer,
        Func<IEnumerable<DayOfYear>, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<ToDoItemDayOfYearSelectorViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(
                view.Items.SelectMany(
                    x => x.Days.Where(y => y.IsSelected).Select(y => new DayOfYear(y.Day, x.Month))
                )
            ),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false),
            setup,
            cancellationToken
        );
    }

    public static ValueTask<Result> ShowDateConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<DateTime, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<CalendarViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.SelectedDate.ThrowIfNullStruct()),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false),
            setup,
            cancellationToken
        );
    }

    public static ValueTask<Result> ShowItemSelectorDialogAsync<TItem>(
        this IDialogViewer dialogViewer,
        Func<TItem, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<ItemSelectorViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.SelectedItem.ThrowIfNull().ThrowIfIsNotCast<TItem>()),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false),
            setup,
            cancellationToken
        );
    }

    public static ValueTask<Result> ShowDateTimeConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<DateTime, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<DateTimeViewModel>? setupCalendar,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync<DateTimeViewModel>(
            viewModel => confirmTask.Invoke(
                viewModel.SelectedDate.ThrowIfNullStruct().Add(viewModel.SelectedTime.ThrowIfNullStruct())
            ),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false),
            viewModel => setupCalendar?.Invoke(viewModel),
            cancellationToken
        );
    }

    public static ValueTask<Result> ShowToDoItemSelectorConfirmDialogAsync(
        this IDialogViewer dialogViewer,
        Func<ToDoSelectorItemNotify, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Action<ToDoItemSelectorViewModel> setup,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmInputDialogAsync(
            view => confirmTask.Invoke(view.SelectedItem.ThrowIfNull()),
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false),
            setup,
            cancellationToken
        );
    }
}
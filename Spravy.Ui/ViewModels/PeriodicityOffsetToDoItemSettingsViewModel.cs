using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class PeriodicityOffsetToDoItemSettingsViewModel : ViewModelBase
{
    private ToDoItemChildrenType childrenType;
    private Guid id;
    private DateOnly dueDate;
    private ushort daysOffset;
    private ushort monthsOffset;
    private ushort weeksOffset;
    private ushort yearsOffset;

    public PeriodicityOffsetToDoItemSettingsViewModel()
    {
        ChangeChildrenTypeCommand = CreateCommandFromTask(TaskWork.Create(ChangeChildrenTypeAsync).RunAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        ChangeDueDateCommand = CreateInitializedCommand(TaskWork.Create(ChangeDueDateAsync).RunAsync);
        ChangeDaysOffsetCommand = CreateInitializedCommand(TaskWork.Create(ChangeDaysOffsetAsync).RunAsync);
        ChangeMonthsOffsetCommand = CreateInitializedCommand(TaskWork.Create(ChangeMonthsOffsetAsync).RunAsync);
        ChangeWeeksOffsetCommand = CreateInitializedCommand(TaskWork.Create(ChangeWeeksOffsetAsync).RunAsync);
        ChangeYearsOffsetCommand = CreateInitializedCommand(TaskWork.Create(ChangeYearsOffsetAsync).RunAsync);
    }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public ToDoItemChildrenType ChildrenType
    {
        get => childrenType;
        set => this.RaiseAndSetIfChanged(ref childrenType, value);
    }

    public DateOnly DueDate
    {
        get => dueDate;
        set => this.RaiseAndSetIfChanged(ref dueDate, value);
    }

    public ushort DaysOffset
    {
        get => daysOffset;
        set => this.RaiseAndSetIfChanged(ref daysOffset, value);
    }

    public ushort MonthsOffset
    {
        get => monthsOffset;
        set => this.RaiseAndSetIfChanged(ref monthsOffset, value);
    }

    public ushort WeeksOffset
    {
        get => weeksOffset;
        set => this.RaiseAndSetIfChanged(ref weeksOffset, value);
    }

    public ushort YearsOffset
    {
        get => yearsOffset;
        set => this.RaiseAndSetIfChanged(ref yearsOffset, value);
    }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    public IRefresh? Refresh { get; set; }

    public ICommand ChangeChildrenTypeCommand { get; }
    public ICommand InitializedCommand { get; }
    public ICommand ChangeDueDateCommand { get; }
    public ICommand ChangeDaysOffsetCommand { get; }
    public ICommand ChangeMonthsOffsetCommand { get; }
    public ICommand ChangeWeeksOffsetCommand { get; }
    public ICommand ChangeYearsOffsetCommand { get; }

    private async Task ChangeYearsOffsetAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowNumberUInt16InputDialogAsync(
                async value =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemYearsOffsetAsync(Id, value, cancellationToken)
                        .ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                    await Refresh.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                calendar => calendar.Value = YearsOffset,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task ChangeWeeksOffsetAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowNumberUInt16InputDialogAsync(
                async value =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemWeeksOffsetAsync(Id, value, cancellationToken)
                        .ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                    await Refresh.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                calendar => calendar.Value = WeeksOffset,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task ChangeMonthsOffsetAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowNumberUInt16InputDialogAsync(
                async value =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemMonthsOffsetAsync(Id, value, cancellationToken)
                        .ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                    await Refresh.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                calendar => calendar.Value = MonthsOffset,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task ChangeDaysOffsetAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowNumberUInt16InputDialogAsync(
                async value =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemDaysOffsetAsync(Id, value, cancellationToken)
                        .ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                    await Refresh.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                calendar => calendar.Value = DaysOffset,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task ChangeDueDateAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowDateConfirmDialogAsync(
                async value =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemDueDateAsync(Id, value.ToDateOnly(), cancellationToken)
                        .ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                    await Refresh.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                calendar => calendar.SelectedDate = DueDate.ToDateTime(),
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var setting = await ToDoService.GetPeriodicityOffsetToDoItemSettingsAsync(Id, cancellationToken)
            .ConfigureAwait(false);

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                ChildrenType = setting.ChildrenType;
                DueDate = setting.DueDate;
                MonthsOffset = setting.MonthsOffset;
                YearsOffset = setting.YearsOffset;
                DaysOffset = setting.DaysOffset;
                WeeksOffset = setting.WeeksOffset;
            }
        );
    }

    private async Task ChangeChildrenTypeAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowItemSelectorDialogAsync<ToDoItemChildrenType>(
                async item =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemChildrenTypeAsync(Id, item, cancellationToken)
                        .ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                    await Refresh.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                viewModel =>
                {
                    viewModel.Items.AddRange(Enum.GetValues<ToDoItemChildrenType>().OfType<object>());
                    viewModel.SelectedItem = ChildrenType;
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }
}
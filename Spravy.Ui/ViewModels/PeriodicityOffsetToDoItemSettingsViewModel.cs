using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class PeriodicityOffsetToDoItemSettingsViewModel : ViewModelBase,
    IToDoChildrenTypeProperty,
    IToDoDueDateProperty,
    IToDoDaysOffsetProperty,
    IToDoMonthsOffsetProperty,
    IToDoWeeksOffsetProperty,
    IToDoYearsOffsetProperty,
    IIsRequiredCompleteInDueDateProperty,
    IApplySettings
{
    private ToDoItemChildrenType childrenType;
    private Guid id;
    private DateOnly dueDate;
    private ushort daysOffset;
    private ushort monthsOffset;
    private ushort weeksOffset;
    private ushort yearsOffset;
    private bool isRequiredCompleteInDueDate;

    public PeriodicityOffsetToDoItemSettingsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<ToDoItemChildrenType> ChildrenTypes { get; } = new(Enum.GetValues<ToDoItemChildrenType>());

    public bool IsRequiredCompleteInDueDate
    {
        get => isRequiredCompleteInDueDate;
        set => this.RaiseAndSetIfChanged(ref isRequiredCompleteInDueDate, value);
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

    public ICommand InitializedCommand { get; }

    private Task InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
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
                IsRequiredCompleteInDueDate = setting.IsRequiredCompleteInDueDate;
            }
        );
    }

    public Task ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(
            ToDoService.UpdateToDoItemDaysOffsetAsync(Id, DaysOffset, cancellationToken),
            ToDoService.UpdateToDoItemWeeksOffsetAsync(Id, WeeksOffset, cancellationToken),
            ToDoService.UpdateToDoItemYearsOffsetAsync(Id, YearsOffset, cancellationToken),
            ToDoService.UpdateToDoItemMonthsOffsetAsync(Id, MonthsOffset, cancellationToken),
            ToDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, cancellationToken),
            ToDoService.UpdateToDoItemDueDateAsync(Id, DueDate, cancellationToken),
            ToDoService
                .UpdateToDoItemIsRequiredCompleteInDueDateAsync(Id, IsRequiredCompleteInDueDate, cancellationToken)
        );
    }
}
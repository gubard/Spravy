using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using Avalonia.Collections;
using Ninject;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Models;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

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
    public PeriodicityOffsetToDoItemSettingsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<ToDoItemChildrenType> ChildrenTypes { get; } = new(Enum.GetValues<ToDoItemChildrenType>());

    [Reactive]
    public bool IsRequiredCompleteInDueDate { get; set; }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public ToDoItemChildrenType ChildrenType { get; set; }

    [Reactive]
    public DateOnly DueDate { get; set; }

    [Reactive]
    public ushort DaysOffset { get; set; }

    [Reactive]
    public ushort MonthsOffset { get; set; }

    [Reactive]
    public ushort WeeksOffset { get; set; }

    [Reactive]
    public ushort YearsOffset { get; set; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    public ICommand InitializedCommand { get; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetPeriodicityOffsetToDoItemSettingsAsync(Id, cancellationToken)
            .IfSuccessAsync(
                setting => this.InvokeUIBackgroundAsync(
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
                ),
                cancellationToken
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse.IfSuccessAllAsync(
            cancellationToken,
            () => ToDoService.UpdateToDoItemDaysOffsetAsync(Id, DaysOffset, cancellationToken),
            () => ToDoService.UpdateToDoItemWeeksOffsetAsync(Id, WeeksOffset, cancellationToken),
            () => ToDoService.UpdateToDoItemYearsOffsetAsync(Id, YearsOffset, cancellationToken),
            () => ToDoService.UpdateToDoItemMonthsOffsetAsync(Id, MonthsOffset, cancellationToken),
            () => ToDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, cancellationToken),
            () => ToDoService.UpdateToDoItemDueDateAsync(Id, DueDate, cancellationToken),
            () => ToDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                Id,
                IsRequiredCompleteInDueDate,
                cancellationToken
            )
        );
    }
}
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.ViewModels;

public class PlannedToDoItemSettingsViewModel : ViewModelBase,
    IToDoChildrenTypeProperty,
    IToDoDueDateProperty,
    IIsRequiredCompleteInDueDateProperty,
    IApplySettings
{
    private ToDoItemChildrenType childrenType;
    private Guid id;
    private DateOnly dueDate;
    private bool isRequiredCompleteInDueDate;

    public PlannedToDoItemSettingsViewModel()
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

    [Inject]
    public required IToDoService ToDoService { get; set; }

    public ICommand InitializedCommand { get; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetPlannedToDoItemSettingsAsync(Id, cancellationToken)
            .IfSuccessAsync(
                setting => this.InvokeUIBackgroundAsync(
                    () =>
                    {
                        ChildrenType = setting.ChildrenType;
                        DueDate = setting.DueDate;
                        IsRequiredCompleteInDueDate = setting.IsRequiredCompleteInDueDate;
                    }
                )
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse.IfSuccessAllAsync(
            () => ToDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, cancellationToken),
            () => ToDoService.UpdateToDoItemDueDateAsync(Id, DueDate, cancellationToken),
            () => ToDoService
                .UpdateToDoItemIsRequiredCompleteInDueDateAsync(Id, IsRequiredCompleteInDueDate, cancellationToken)
        );
    }
}
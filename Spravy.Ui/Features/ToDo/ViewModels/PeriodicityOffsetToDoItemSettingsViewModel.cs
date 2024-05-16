namespace Spravy.Ui.Features.ToDo.ViewModels;

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

    [Inject]
    public required IToDoService ToDoService { get; set; }

    public ICommand InitializedCommand { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.UpdateToDoItemDaysOffsetAsync(Id, DaysOffset, cancellationToken)
           .IfSuccessAsync(() => ToDoService.UpdateToDoItemWeeksOffsetAsync(Id, WeeksOffset, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => ToDoService.UpdateToDoItemYearsOffsetAsync(Id, YearsOffset, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => ToDoService.UpdateToDoItemMonthsOffsetAsync(Id, MonthsOffset, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => ToDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => ToDoService.UpdateToDoItemDueDateAsync(Id, DueDate, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => ToDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(Id, IsRequiredCompleteInDueDate, cancellationToken),
                cancellationToken);
    }

    [Reactive]
    public bool IsRequiredCompleteInDueDate { get; set; }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public ToDoItemChildrenType ChildrenType { get; set; }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetPeriodicityOffsetToDoItemSettingsAsync(Id, cancellationToken)
           .IfSuccessAsync(setting => this.InvokeUiBackgroundAsync(() =>
            {
                ChildrenType = setting.ChildrenType;
                DueDate = setting.DueDate;
                MonthsOffset = setting.MonthsOffset;
                YearsOffset = setting.YearsOffset;
                DaysOffset = setting.DaysOffset;
                WeeksOffset = setting.WeeksOffset;
                IsRequiredCompleteInDueDate = setting.IsRequiredCompleteInDueDate;
                
                return Result.Success;
            }), cancellationToken);
    }

    [Reactive]
    public ushort DaysOffset { get; set; }

    [Reactive]
    public DateOnly DueDate { get; set; }

    [Reactive]
    public ushort MonthsOffset { get; set; }

    [Reactive]
    public ushort WeeksOffset { get; set; }

    [Reactive]
    public ushort YearsOffset { get; set; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }
}
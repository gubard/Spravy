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
    private readonly IToDoService toDoService;
    
    public PeriodicityOffsetToDoItemSettingsViewModel(IToDoService toDoService, IErrorHandler errorHandler)
    {
        this.toDoService = toDoService;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler);
    }

    public SpravyCommand InitializedCommand { get; }
    
    [Reactive]
    public bool IsRequiredCompleteInDueDate { get; set; }
    
    [Reactive]
    public Guid Id { get; set; }
    
    [Reactive]
    public ToDoItemChildrenType ChildrenType { get; set; }
    
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

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return toDoService.UpdateToDoItemDaysOffsetAsync(Id, DaysOffset, cancellationToken)
           .IfSuccessAsync(() => toDoService.UpdateToDoItemWeeksOffsetAsync(Id, WeeksOffset, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => toDoService.UpdateToDoItemYearsOffsetAsync(Id, YearsOffset, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => toDoService.UpdateToDoItemMonthsOffsetAsync(Id, MonthsOffset, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => toDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => toDoService.UpdateToDoItemDueDateAsync(Id, DueDate, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(Id, IsRequiredCompleteInDueDate, cancellationToken),
                cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return toDoService.GetPeriodicityOffsetToDoItemSettingsAsync(Id, cancellationToken)
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

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }
}
namespace Spravy.Ui.Features.ToDo.ViewModels;

public class PlannedToDoItemSettingsViewModel : ViewModelBase,
    IToDoChildrenTypeProperty,
    IToDoDueDateProperty,
    IIsRequiredCompleteInDueDateProperty,
    IApplySettings
{
    private readonly IToDoService toDoService;
    
    public PlannedToDoItemSettingsViewModel(IToDoService toDoService, IErrorHandler errorHandler)
    {
        this.toDoService = toDoService;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler);
    }
    
    public SpravyCommand InitializedCommand { get; }
    
    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return toDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, cancellationToken)
           .IfSuccessAsync(() => toDoService.UpdateToDoItemDueDateAsync(Id, DueDate, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(
                () => toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(Id, IsRequiredCompleteInDueDate,
                    cancellationToken), cancellationToken);
    }
    
    [Reactive]
    public bool IsRequiredCompleteInDueDate { get; set; }
    
    [Reactive]
    public Guid Id { get; set; }
    
    [Reactive]
    public ToDoItemChildrenType ChildrenType { get; set; }
    
    [Reactive]
    public DateOnly DueDate { get; set; }
    
    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return toDoService.GetPlannedToDoItemSettingsAsync(Id, cancellationToken)
           .IfSuccessAsync(setting => this.InvokeUiBackgroundAsync(() =>
            {
                ChildrenType = setting.ChildrenType;
                DueDate = setting.DueDate;
                IsRequiredCompleteInDueDate = setting.IsRequiredCompleteInDueDate;
                
                return Result.Success;
            }), cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }
}
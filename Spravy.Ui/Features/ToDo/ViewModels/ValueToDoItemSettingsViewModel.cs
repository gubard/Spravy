namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ValueToDoItemSettingsViewModel : ViewModelBase, IToDoChildrenTypeProperty, IApplySettings
{
    private readonly IToDoService toDoService;
    
    public ValueToDoItemSettingsViewModel(IToDoService toDoService, IErrorHandler errorHandler)
    {
        this.toDoService = toDoService;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler);
    }
    
    public SpravyCommand InitializedCommand { get; }
    
    [Reactive]
    public Guid Id { get; set; }
    
    [Reactive]
    public ToDoItemChildrenType ChildrenType { get; set; }
    
    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return toDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return toDoService.GetValueToDoItemSettingsAsync(Id, cancellationToken)
           .IfSuccessAsync(setting => this.InvokeUiBackgroundAsync(() =>
            {
                ChildrenType = setting.ChildrenType;
                
                return Result.Success;
            }), cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }
}
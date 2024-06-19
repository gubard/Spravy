namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ReferenceToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    
    public ReferenceToDoItemSettingsViewModel(
        ToDoItemSelectorViewModel toDoItemSelector,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        ToDoItemSelector = toDoItemSelector;
        this.toDoService = toDoService;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);
    }
    
    public SpravyCommand InitializedCommand { get; }
    public ToDoItemSelectorViewModel ToDoItemSelector { get; }
    
    [Reactive]
    public Guid ToDoItemId { get; set; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return toDoService.GetReferenceToDoItemSettingsAsync(ToDoItemId, cancellationToken)
           .IfSuccessAsync(setting =>
            {
                ToDoItemSelector.IgnoreIds = new([ToDoItemId,]);
                ToDoItemSelector.DefaultSelectedItemId = setting.ReferenceId;
                
                return Result.Success;
            }, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return toDoService.UpdateReferenceToDoItemAsync(ToDoItemId, ToDoItemSelector.SelectedItem.ThrowIfNull().Id,
            cancellationToken);
    }
}
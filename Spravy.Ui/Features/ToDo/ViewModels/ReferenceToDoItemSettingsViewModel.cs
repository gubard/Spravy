namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ReferenceToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    public ReferenceToDoItemSettingsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required ToDoItemSelectorViewModel ToDoItemSelector { get; init; }
    
    [Reactive]
    public Guid ToDoItemId { get; set; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetReferenceToDoItemSettingsAsync(ToDoItemId, cancellationToken)
           .IfSuccessAsync(setting =>
            {
                ToDoItemSelector.IgnoreIds.Add(ToDoItemId);
                ToDoItemSelector.DefaultSelectedItemId = setting.ReferenceId;
                
                return Result.Success;
            }, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.UpdateReferenceToDoItemAsync(ToDoItemId, ToDoItemSelector.SelectedItem.ThrowIfNull().Id,
            cancellationToken);
    }
}
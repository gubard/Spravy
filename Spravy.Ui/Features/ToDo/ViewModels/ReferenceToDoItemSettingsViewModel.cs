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
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return toDoService.GetReferenceToDoItemSettingsAsync(ToDoItemId, ct)
           .IfSuccessAsync(setting =>
            {
                ToDoItemSelector.IgnoreIds = new([ToDoItemId,]);

                if (setting.ReferenceId.TryGetValue(out var referenceId))
                {
                    ToDoItemSelector.DefaultSelectedItemId = referenceId;
                }

                return Result.Success;
            }, ct);
    }
    
    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.UpdateReferenceToDoItemAsync(ToDoItemId, ToDoItemSelector.SelectedItem.ThrowIfNull().Id,
            ct);
    }
}
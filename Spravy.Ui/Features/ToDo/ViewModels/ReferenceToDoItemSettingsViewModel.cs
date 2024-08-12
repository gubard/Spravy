namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ReferenceToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;

    public ReferenceToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        ToDoItemSelectorViewModel toDoItemSelector,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService
    )
    {
        ToDoItemSelector = toDoItemSelector;
        this.toDoService = toDoService;
        this.toDoUiService = toDoUiService;
        Item = item;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public ToDoItemEntityNotify Item { get; }
    public SpravyCommand InitializedCommand { get; }
    public ToDoItemSelectorViewModel ToDoItemSelector { get; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return toDoUiService.UpdateItemAsync(Item, ct);
    }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return ToDoItemSelector
            .SelectedItem.IfNotNull(nameof(ToDoItemSelector.SelectedItem))
            .IfSuccessAsync(
                selectedItem =>
                    toDoService.UpdateReferenceToDoItemAsync(Item.Id, selectedItem.Id, ct),
                ct
            );
    }
}

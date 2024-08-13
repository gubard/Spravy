namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ReferenceToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;

    public ReferenceToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        ToDoItemSelectorViewModel toDoItemSelector,
        IToDoService toDoService
    )
    {
        ToDoItemSelector = toDoItemSelector;
        this.toDoService = toDoService;
        Item = item;
    }

    public ToDoItemEntityNotify Item { get; }
    public ToDoItemSelectorViewModel ToDoItemSelector { get; }

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

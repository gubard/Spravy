namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ReferenceToDoItemSettingsViewModel : ViewModelBase, IEditToDoItems
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;

    public ReferenceToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        ToDoItemSelectorViewModel toDoItemSelector,
        IToDoService toDoService,
        IToDoCache toDoCache
    )
    {
        ToDoItemSelector = toDoItemSelector;
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        Item = item;
    }

    public ToDoItemEntityNotify Item { get; }
    public ToDoItemSelectorViewModel ToDoItemSelector { get; }

    public EditToDoItems GetEditToDoItems()
    {
        return new EditToDoItems()
            .SetIds(new[] { Item.Id })
            .SetReferenceId(new((ToDoItemSelector.SelectedItem?.Id).ToOption()));
    }

    public Result UpdateItemUi()
    {
        return ToDoItemSelector
            .GetSelectedItem()
            .IfSuccess(selected => toDoCache.GetToDoItem(selected.Id))
            .IfSuccess(reference =>
            {
                Item.Reference = reference;

                return Result.Success;
            });
    }
}

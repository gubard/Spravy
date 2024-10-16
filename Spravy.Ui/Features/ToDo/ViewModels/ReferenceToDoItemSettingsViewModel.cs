namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ReferenceToDoItemSettingsViewModel : ViewModelBase, IToDoItemSettings
{
    private readonly IToDoCache toDoCache;

    public ReferenceToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        ToDoItemSelectorViewModel toDoItemSelector,
        IToDoCache toDoCache
    )
    {
        ToDoItemSelector = toDoItemSelector;
        this.toDoCache = toDoCache;
        Item = item;
    }

    public ToDoItemEntityNotify Item { get; }
    public ToDoItemSelectorViewModel ToDoItemSelector { get; }
    public string ViewId => TypeCache<ReferenceToDoItemSettingsViewModel>.Type.Name;

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

    public Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}

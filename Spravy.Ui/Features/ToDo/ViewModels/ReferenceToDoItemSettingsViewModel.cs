namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ReferenceToDoItemSettingsViewModel : ViewModelBase, IApplySettings
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

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return ToDoItemSelector
            .SelectedItem.IfNotNull(nameof(ToDoItemSelector.SelectedItem))
            .IfSuccessAsync(
                selectedItem =>
                    toDoService.UpdateReferenceToDoItemAsync(Item.Id, selectedItem.Id, ct),
                ct
            );
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

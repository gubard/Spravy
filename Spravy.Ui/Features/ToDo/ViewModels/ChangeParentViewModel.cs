namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ChangeParentViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IToDoService toDoService;

    public ChangeParentViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IToDoService toDoService,
        ToDoItemSelectorViewModel toDoItemSelectorViewModel
    )
        : base(editItem, editItems)
    {
        this.toDoService = toDoService;
        ToDoItemSelectorViewModel = toDoItemSelectorViewModel;
    }

    public override string ViewId => TypeCache<ChangeParentViewModel>.Type.Name;
    public ToDoItemSelectorViewModel ToDoItemSelectorViewModel { get; }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        if (ToDoItemSelectorViewModel.SelectedItem is null)
        {
            return toDoService.EditToDoItemsAsync(
                new EditToDoItems().SetIds(ResultIds).SetParentId(new(new())),
                ct
            );
        }

        return toDoService.EditToDoItemsAsync(
            new EditToDoItems()
                .SetIds(ResultIds)
                .SetParentId(new(new(ToDoItemSelectorViewModel.SelectedItem.Id))),
            ct
        );
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class CloneViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IToDoCache toDoCache;

    private readonly IToDoService toDoService;

    public CloneViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        ToDoItemSelectorViewModel toDoItemSelectorViewModel,
        IToDoService toDoService,
        IToDoCache toDoCache
    ) : base(editItem, editItems)
    {
        ToDoItemSelectorViewModel = toDoItemSelectorViewModel;
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
    }

    public override string ViewId => TypeCache<CloneViewModel>.Type.Name;

    public ToDoItemSelectorViewModel ToDoItemSelectorViewModel { get; }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        if (ToDoItemSelectorViewModel.SelectedItem is null)
        {
            return toDoService.CloneToDoItemAsync(ResultIds, new(), ct).ToResultOnlyAsync();
        }

        return toDoService.CloneToDoItemAsync(ResultIds, new(ToDoItemSelectorViewModel.SelectedItem.Id), ct)
           .ToResultOnlyAsync();
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }

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
}
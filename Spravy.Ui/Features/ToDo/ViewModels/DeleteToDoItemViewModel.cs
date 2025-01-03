namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class DeleteToDoItemViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;

    [ObservableProperty]
    private string childrenText = string.Empty;

    public DeleteToDoItemViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IToDoService toDoService,
        IToDoUiService toDoUiService
    ) : base(editItem, editItems)
    {
        this.toDoService = toDoService;
        this.toDoUiService = toDoUiService;
    }

    public override string ViewId =>
        EditItem.TryGetValue(out var editItem)
            ? $"{TypeCache<AddToDoItemViewModel>.Name}:{editItem.Id}"
            : TypeCache<AddToDoItemViewModel>.Name;

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.DeleteToDoItemsAsync(ResultIds, ct);
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
        var statuses = UiHelper.ToDoItemStatuses;

        return toDoUiService.GetRequest(
                GetToDo.WithDefaultItems
                   .SetIsBookmarkItems(true)
                   .SetIsFavoriteItems(true)
                   .SetToStringItem(
                        new(ResultIds, statuses)
                    ),
                ct
            )
           .IfSuccessAsync(
                response => this.InvokeUiBackgroundAsync(
                    () =>
                    {
                        ChildrenText = response.ToStringItems.Select(x => x.Text).JoinString(Environment.NewLine);

                        return Result.Success;
                    }
                ),
                ct
            );
    }
}
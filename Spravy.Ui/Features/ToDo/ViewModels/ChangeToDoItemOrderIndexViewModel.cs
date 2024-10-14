namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ChangeToDoItemOrderIndexViewModel
    : ToDoItemEditIdViewModel,
        IToDoItemsView,
        IApplySettings
{
    private readonly AvaloniaList<ToDoItemEntityNotify> items;
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private ToDoItemEntityNotify? selectedItem;

    [ObservableProperty]
    private bool isAfter = true;

    public ChangeToDoItemOrderIndexViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IToDoService toDoService
    )
        : base(editItem, editItems)
    {
        items = new();
        this.toDoService = toDoService;
    }

    public IAvaloniaReadOnlyList<ToDoItemEntityNotify> Items => items;

    public Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> newItems)
    {
        return items.UpdateUi(newItems).ToResultOnly();
    }

    public Result AddOrUpdateUi(ToDoItemEntityNotify item)
    {
        return new(new NotImplementedError(nameof(AddOrUpdateUi)));
    }

    public Result RemoveUi(ToDoItemEntityNotify item)
    {
        items.Remove(item);

        return Result.Success;
    }

    public override string ViewId
    {
        get => $"{TypeCache<DialogableViewModelBase>.Type}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return SelectedItem
            .IfNotNull(nameof(SelectedItem))
            .IfSuccessAsync(
                si =>
                    ResultIds
                        .ToResult()
                        .IfSuccessForEach(x =>
                            new UpdateOrderIndexToDoItemOptions(x, si.Id, IsAfter).ToResult()
                        )
                        .IfSuccessAsync(
                            options => toDoService.UpdateToDoItemOrderIndexAsync(options, ct),
                            ct
                        ),
                ct
            );
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}

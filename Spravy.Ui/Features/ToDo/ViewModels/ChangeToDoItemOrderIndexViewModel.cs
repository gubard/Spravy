namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ChangeToDoItemOrderIndexViewModel : ToDoItemEditIdViewModel, IToDoItemsView, IApplySettings
{
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private bool isAfter = true;

    [ObservableProperty]
    private ToDoItemEntityNotify? selectedItem;

    public ChangeToDoItemOrderIndexViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IToDoService toDoService
    ) : base(editItem, editItems)
    {
        this.toDoService = toDoService;
    }

    public AvaloniaList<ToDoItemEntityNotify> Items { get; } = new();

    public override string ViewId => $"{TypeCache<DialogableViewModelBase>.Type}";

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return SelectedItem.IfNotNull(nameof(SelectedItem))
           .IfSuccessAsync(
                si => ResultIds.ToResult()
                   .IfSuccessForEach(x => new UpdateOrderIndexToDoItemOptions(x, si.Id, IsAfter).ToResult())
                   .IfSuccessAsync(options => toDoService.UpdateToDoItemOrderIndexAsync(options, ct), ct),
                ct
            );
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Result SetItemsUi(ReadOnlyMemory<ToDoItemEntityNotify> newItems)
    {
        return Items.UpdateUi(newItems).ToResultOnly();
    }

    public Result AddOrUpdateUi(ReadOnlyMemory<ToDoItemEntityNotify> _)
    {
        return new(new NotImplementedError(nameof(AddOrUpdateUi)));
    }

    public Result RemoveUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        Items.RemoveAll(items.ToArray());

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
}
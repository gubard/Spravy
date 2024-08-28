namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ChangeToDoItemOrderIndexViewModel : DialogableViewModelBase, IToDoItemsView
{
    private readonly AvaloniaList<ToDoItemEntityNotify> items;
    private readonly IToDoUiService toDoUiService;

    [ObservableProperty]
    private ToDoItemEntityNotify? selectedItem;

    [ObservableProperty]
    private bool isAfter = true;

    public ChangeToDoItemOrderIndexViewModel(
        ToDoItemEntityNotify? item,
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService
    )
    {
        this.items = new(items.ToArray());
        this.toDoUiService = toDoUiService;
        Item = item;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public ToDoItemEntityNotify? Item { get; }
    public SpravyCommand InitializedCommand { get; }
    public IAvaloniaReadOnlyList<ToDoItemEntityNotify> Items => items;

    private Cvtar InitializedAsync(CancellationToken ct)
    {
        if (Item is not null && items.IsEmpty())
        {
            return toDoUiService.UpdateSiblingsAsync(Item, this, ct);
        }

        return toDoUiService.UpdateItemsAsync(items.ToArray(), ct);
    }

    public Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> newItems)
    {
        return this.PostUiBackground(() => items.UpdateUi(newItems), CancellationToken.None);
    }

    public Result AddOrUpdateUi(ToDoItemEntityNotify item)
    {
        return new(new NotImplementedError(nameof(AddOrUpdateUi)));
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
}

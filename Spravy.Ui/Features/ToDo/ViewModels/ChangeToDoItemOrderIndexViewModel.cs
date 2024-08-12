namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ChangeToDoItemOrderIndexViewModel : ViewModelBase
{
    private readonly AvaloniaList<ToDoItemEntityNotify> items = new();
    private readonly IToDoUiService toDoUiService;

    [ObservableProperty]
    private ToDoItemEntityNotify? selectedItem;

    [ObservableProperty]
    private bool isAfter = true;

    public ChangeToDoItemOrderIndexViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService
    )
    {
        this.items.AddRange(items.ToArray());
        this.toDoUiService = toDoUiService;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }
    public IAvaloniaReadOnlyList<ToDoItemEntityNotify> Items => items;

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return toDoUiService.UpdateItemsAsync(Items.ToArray(), ct);
    }
}

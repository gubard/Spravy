namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ChangeToDoItemOrderIndexViewModel : ViewModelBase
{
    private readonly IToDoUiService toDoUiService;

    [ObservableProperty]
    private ToDoItemEntityNotify? selectedItem;

    [ObservableProperty]
    private bool isAfter = true;

    public ChangeToDoItemOrderIndexViewModel(
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService
    )
    {
        this.toDoUiService = toDoUiService;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }
    public ToDoItemEntityNotify? Item { get; set; }
    public AvaloniaList<ToDoItemEntityNotify> Items { get; } = new();

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        if (Items.IsEmpty())
        {
            return Item.IfNotNull(nameof(Item))
                .IfSuccessAsync(i => toDoUiService.GetSiblingsAsync(i, ct), ct)
                .IfSuccessAsync(
                    items => this.PostUiBackground(() => Items.UpdateUi(items), ct),
                    ct
                );
        }

        return this.PostUiBackground(() => Items.UpdateUi(Items), ct)
            .IfSuccessAsync(() => toDoUiService.UpdateItemsAsync(Items.ToArray(), ct), ct);
    }
}

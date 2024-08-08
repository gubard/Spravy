namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ChangeToDoItemOrderIndexViewModel : ViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;

    [ObservableProperty]
    private ToDoItemEntityNotify? selectedItem;

    [ObservableProperty]
    private bool isAfter = true;

    public ChangeToDoItemOrderIndexViewModel(
        IToDoService toDoService,
        IToDoCache toDoCache,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
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
                .IfSuccessAsync(i => toDoService.GetSiblingsAsync(i.Id, ct), ct)
                .IfSuccessAsync(
                    items =>
                        this.PostUiBackground(
                            () =>
                            {
                                Items.Clear();

                                return items
                                    .ToResult()
                                    .IfSuccessForEach(item =>
                                        toDoCache
                                            .UpdateUi(item)
                                            .IfSuccess(i =>
                                            {
                                                Items.Add(i);

                                                return Result.Success;
                                            })
                                    );
                            },
                            ct
                        ),
                    ct
                );
        }

        return this.PostUiBackground(
                () =>
                {
                    Items.Update(Items);

                    return Result.Success;
                },
                ct
            )
            .IfSuccess(() => Items.Select(x => x.Id).ToArray().ToReadOnlyMemory().ToResult())
            .IfSuccessForEachAsync(id => toDoService.GetToDoItemAsync(id, ct), ct)
            .IfSuccessForEachAsync(id => toDoCache.UpdateUi(id), ct)
            .ToResultOnlyAsync();
    }
}

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

    public ReadOnlyMemory<Guid> ChangeToDoItemOrderIndexIds { get; set; } =
        ReadOnlyMemory<Guid>.Empty;

    public SpravyCommand InitializedCommand { get; }
    public Guid Id { get; set; }
    public AvaloniaList<ToDoItemEntityNotify> Items { get; } = new();

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        if (ChangeToDoItemOrderIndexIds.IsEmpty)
        {
            return toDoService
                .GetSiblingsAsync(Id, ct)
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

        return ChangeToDoItemOrderIndexIds
            .ToResult()
            .IfSuccessForEach(id => toDoCache.GetToDoItem(id))
            .IfSuccess(items =>
                this.PostUiBackground(
                    () =>
                    {
                        Items.Update(items.ToArray());

                        return Result.Success;
                    },
                    ct
                )
            )
            .ToValueTaskResult()
            .ConfigureAwait(false);
    }
}

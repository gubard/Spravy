namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class DeleteToDoItemViewModel : DialogableViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;
    private readonly AvaloniaList<ToDoItemEntityNotify> items = new();

    [ObservableProperty]
    private string childrenText = string.Empty;

    public DeleteToDoItemViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        IToDoService toDoService,
        IToDoUiService toDoUiService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        this.toDoUiService = toDoUiService;
        Item = item;
        this.items.AddRange(items.ToArray());

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public DeleteToDoItemViewModel(
        ToDoItemEntityNotify item,
        IToDoService toDoService,
        IToDoUiService toDoUiService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        this.toDoUiService = toDoUiService;
        Item = item;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public DeleteToDoItemViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        IToDoService toDoService,
        IToDoUiService toDoUiService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        this.toDoUiService = toDoUiService;
        Item = null;
        this.items.AddRange(items.ToArray());

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public ToDoItemEntityNotify? Item { get; }
    public SpravyCommand InitializedCommand { get; }
    public IAvaloniaReadOnlyList<ToDoItemEntityNotify> Items => items;

    public override string ViewId
    {
        get => $"{TypeCache<DeleteToDoItemViewModel>.Type}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    private Cvtar InitializedAsync(CancellationToken ct)
    {
        var statuses = UiHelper.ToDoItemStatuses;

        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => Item is null ? Result.AwaitableSuccess : toDoUiService.UpdateItemAsync(Item, ct),
            () =>
            {
                if (Items.IsEmpty())
                {
                    return Item.IfNotNull(nameof(Item))
                        .IfSuccessAsync(
                            i =>
                                toDoService
                                    .ToDoItemToStringAsync(new(statuses, i.Id), ct)
                                    .IfSuccessAsync(
                                        text =>
                                            this.PostUiBackground(
                                                () =>
                                                {
                                                    ChildrenText = text;

                                                    return Result.Success;
                                                },
                                                ct
                                            ),
                                        ct
                                    ),
                            ct
                        );
                }

                return Items
                    .ToArray()
                    .ToReadOnlyMemory()
                    .ToResult()
                    .IfSuccessForEachAsync(
                        i =>
                            toDoService
                                .ToDoItemToStringAsync(new(statuses, i.Id), ct)
                                .IfSuccessAsync(
                                    str =>
                                        $"{i.Name}{Environment.NewLine} {str.Split(Environment.NewLine).JoinString($"{Environment.NewLine} ")}".ToResult(),
                                    ct
                                ),
                        ct
                    )
                    .IfSuccessAsync(
                        values =>
                        {
                            var text = string.Join(Environment.NewLine, values.ToArray());

                            return this.PostUiBackground(
                                () =>
                                {
                                    ChildrenText = text;

                                    return Result.Success;
                                },
                                ct
                            );
                        },
                        ct
                    );
            }
        );
    }
}

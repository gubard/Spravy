namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class DeleteToDoItemViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private string childrenText = string.Empty;

    public DeleteToDoItemViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
        : base(editItem, editItems)
    {
        this.toDoService = toDoService;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }

    public override string ViewId
    {
        get =>
            EditItem.TryGetValue(out var editItem)
                ? $"{TypeCache<AddToDoItemViewModel>.Type.Name}:{editItem.Id}"
                : $"{TypeCache<AddToDoItemViewModel>.Type.Name}";
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

        return ResultItems
            .ToResult()
            .IfSuccessForEachAsync(
                id =>
                    toDoService
                        .ToDoItemToStringAsync(
                            new ToDoItemToStringOptions[] { new(statuses, id.Id) },
                            ct
                        )
                        .IfSuccessAsync(
                            str =>
                                $"{id.Name}{Environment.NewLine} {str.Split(Environment.NewLine).JoinString($"{Environment.NewLine} ")}".ToResult(),
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

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.DeleteToDoItemsAsync(ResultIds, ct);
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class PlannedToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;

    public PlannedToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService
    )
    {
        Item = item;
        this.toDoService = toDoService;
        this.toDoUiService = toDoUiService;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    private ToDoItemEntityNotify Item { get; }
    public SpravyCommand InitializedCommand { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService
            .UpdateToDoItemChildrenTypeAsync(Item.Id, Item.ChildrenType, ct)
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemDueDateAsync(Item.Id, Item.DueDate, ct),
                ct
            )
            .IfSuccessAsync(
                () =>
                    toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                        Item.Id,
                        Item.IsRequiredCompleteInDueDate,
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return toDoUiService.UpdateItemAsync(Item, ct);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return RefreshAsync(ct);
    }
}

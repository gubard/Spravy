namespace Spravy.Ui.Features.ToDo.ViewModels;

public class PeriodicityOffsetToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;

    public PeriodicityOffsetToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService
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

    public ToDoItemEntityNotify Item { get; }
    public SpravyCommand InitializedCommand { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService
            .UpdateToDoItemDaysOffsetAsync(Item.Id, Item.DaysOffset, ct)
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemWeeksOffsetAsync(Item.Id, Item.WeeksOffset, ct),
                ct
            )
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemYearsOffsetAsync(Item.Id, Item.YearsOffset, ct),
                ct
            )
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemMonthsOffsetAsync(Item.Id, Item.MonthsOffset, ct),
                ct
            )
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemChildrenTypeAsync(Item.Id, Item.ChildrenType, ct),
                ct
            )
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

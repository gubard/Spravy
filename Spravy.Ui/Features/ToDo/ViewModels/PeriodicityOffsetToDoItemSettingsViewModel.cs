namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityOffsetToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;

    [ObservableProperty]
    private ToDoItemEntityNotify? item;

    public PeriodicityOffsetToDoItemSettingsViewModel(
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService
    )
    {
        this.toDoService = toDoService;
        this.toDoUiService = toDoUiService;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return Item.IfNotNull(nameof(Item))
            .IfSuccessAsync(
                i =>
                    toDoService
                        .UpdateToDoItemDaysOffsetAsync(i.Id, i.DaysOffset, ct)
                        .IfSuccessAsync(
                            () =>
                                toDoService.UpdateToDoItemWeeksOffsetAsync(i.Id, i.WeeksOffset, ct),
                            ct
                        )
                        .IfSuccessAsync(
                            () =>
                                toDoService.UpdateToDoItemYearsOffsetAsync(i.Id, i.YearsOffset, ct),
                            ct
                        )
                        .IfSuccessAsync(
                            () =>
                                toDoService.UpdateToDoItemMonthsOffsetAsync(
                                    i.Id,
                                    i.MonthsOffset,
                                    ct
                                ),
                            ct
                        )
                        .IfSuccessAsync(
                            () =>
                                toDoService.UpdateToDoItemChildrenTypeAsync(
                                    i.Id,
                                    i.ChildrenType,
                                    ct
                                ),
                            ct
                        )
                        .IfSuccessAsync(
                            () => toDoService.UpdateToDoItemDueDateAsync(i.Id, i.DueDate, ct),
                            ct
                        )
                        .IfSuccessAsync(
                            () =>
                                toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                                    i.Id,
                                    i.IsRequiredCompleteInDueDate,
                                    ct
                                ),
                            ct
                        ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return Item.IfNotNull(nameof(Item))
            .IfSuccessAsync(i => toDoUiService.UpdateItemAsync(i, ct), ct);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return RefreshAsync(ct);
    }
}

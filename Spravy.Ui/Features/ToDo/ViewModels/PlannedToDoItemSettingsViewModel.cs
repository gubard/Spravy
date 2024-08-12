namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PlannedToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;

    [ObservableProperty]
    private ToDoItemEntityNotify? item;

    public PlannedToDoItemSettingsViewModel(
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
                        .UpdateToDoItemChildrenTypeAsync(i.Id, i.ChildrenType, ct)
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

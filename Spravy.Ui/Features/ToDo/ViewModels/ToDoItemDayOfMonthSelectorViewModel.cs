namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfMonthSelectorViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;

    public ToDoItemDayOfMonthSelectorViewModel(
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
        SelectedDays = new();

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public AvaloniaList<int> SelectedDays { get; }
    public SpravyCommand InitializedCommand { get; }
    public ToDoItemEntityNotify Item { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.UpdateToDoItemMonthlyPeriodicityAsync(
            Item.Id,
            new(SelectedDays.Select(x => (byte)x).ToArray()),
            ct
        );
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return toDoUiService.UpdateItemAsync(Item, ct);
    }
}

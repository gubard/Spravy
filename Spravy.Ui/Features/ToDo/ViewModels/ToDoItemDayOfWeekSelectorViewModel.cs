namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfWeekSelectorViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;

    public ToDoItemDayOfWeekSelectorViewModel(
        ToDoItemEntityNotify item,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService
    )
    {
        this.toDoService = toDoService;
        Item = item;
        this.toDoUiService = toDoUiService;

        Items = new(
            UiHelper.DayOfWeeks.ToArray().Select(x => new DayOfWeekSelectItem { DayOfWeek = x, })
        );

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public AvaloniaList<DayOfWeekSelectItem> Items { get; }
    public SpravyCommand InitializedCommand { get; }
    public ToDoItemEntityNotify Item { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.UpdateToDoItemWeeklyPeriodicityAsync(
            Item.Id,
            new(Items.Where(x => x.IsSelected).Select(x => x.DayOfWeek)),
            ct
        );
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return toDoUiService.UpdateItemAsync(Item, ct);
    }
}

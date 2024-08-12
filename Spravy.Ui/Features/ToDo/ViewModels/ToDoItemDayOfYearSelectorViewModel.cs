namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfYearSelectorViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;

    public ToDoItemDayOfYearSelectorViewModel(
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

        Items = new(
            Enumerable.Range(1, 12).Select(x => new DayOfYearSelectItem { Month = (byte)x, })
        );

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public AvaloniaList<DayOfYearSelectItem> Items { get; }
    public SpravyCommand InitializedCommand { get; }
    public ToDoItemEntityNotify Item { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.UpdateToDoItemAnnuallyPeriodicityAsync(
            Item.Id,
            new(
                Items.SelectMany(x =>
                    x.Days.Where(y => y.IsSelected).Select(y => new DayOfYear(y.Day, x.Month))
                )
            ),
            ct
        );
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return toDoUiService.UpdateItemAsync(Item, ct);
    }
}

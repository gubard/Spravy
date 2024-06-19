namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfWeekSelectorViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;

    public ToDoItemDayOfWeekSelectorViewModel(
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;

        Items = new(Enum.GetValues<DayOfWeek>()
           .Select(x => new DayOfWeekSelectItem
            {
                DayOfWeek = x,
            }));

        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);
    }

    public AvaloniaList<DayOfWeekSelectItem> Items { get; }
    public SpravyCommand InitializedCommand { get; }

    [Reactive]
    public Guid ToDoItemId { get; set; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return toDoService.UpdateToDoItemWeeklyPeriodicityAsync(ToDoItemId,
            new(Items.Where(x => x.IsSelected).Select(x => x.DayOfWeek)), cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return toDoService.GetWeeklyPeriodicityAsync(ToDoItemId, cancellationToken)
           .IfSuccessAsync(weeklyPeriodicity => Result.AwaitableSuccess.IfSuccessAllAsync(cancellationToken,
                Items.Where(x => weeklyPeriodicity.Days.Contains(x.DayOfWeek))
                   .Select<DayOfWeekSelectItem, Func<ConfiguredValueTaskAwaitable<Result>>>(x =>
                    {
                        var y = x;

                        return () => this.InvokeUiBackgroundAsync(() =>
                        {
                            y.IsSelected = true;

                            return Result.Success;
                        });
                    })
                   .ToArray()), cancellationToken);
    }
}
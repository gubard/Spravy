namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfYearSelectorViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;

    public ToDoItemDayOfYearSelectorViewModel(
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;

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

    [Reactive]
    public Guid ToDoItemId { get; set; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.UpdateToDoItemAnnuallyPeriodicityAsync(
            ToDoItemId,
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
        return toDoService
            .GetAnnuallyPeriodicityAsync(ToDoItemId, ct)
            .IfSuccessAsync(
                annuallyPeriodicity =>
                {
                    var items = new List<Func<ConfiguredValueTaskAwaitable<Result>>>();

                    foreach (var item in Items)
                    {
                        foreach (var day in item.Days)
                        {
                            if (
                                annuallyPeriodicity
                                    .Days.Where(x => x.Month == item.Month)
                                    .Select(x => x.Day)
                                    .Contains(day.Day)
                            )
                            {
                                var d = day;

                                items.Add(
                                    () =>
                                        this.InvokeUiBackgroundAsync(() =>
                                        {
                                            d.IsSelected = true;

                                            return Result.Success;
                                        })
                                );
                            }
                        }
                    }

                    return Result.AwaitableSuccess.IfSuccessAllAsync(ct, items.ToArray());
                },
                ct
            );
    }
}

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
    public Guid ToDoItemId { get; set; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.UpdateToDoItemWeeklyPeriodicityAsync(
            ToDoItemId,
            new(Items.Where(x => x.IsSelected).Select(x => x.DayOfWeek)),
            ct
        );
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return toDoService
            .GetWeeklyPeriodicityAsync(ToDoItemId, ct)
            .IfSuccessAsync(
                weeklyPeriodicity =>
                    Result.AwaitableSuccess.IfSuccessAllAsync(
                        ct,
                        Items
                            .Where(x => weeklyPeriodicity.Days.Contains(x.DayOfWeek))
                            .Select<
                                DayOfWeekSelectItem,
                                Func<ConfiguredValueTaskAwaitable<Result>>
                            >(x =>
                            {
                                var y = x;

                                return () =>
                                    this.PostUiBackground(
                                            () =>
                                            {
                                                y.IsSelected = true;

                                                return Result.Success;
                                            },
                                            ct
                                        )
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false);
                            })
                            .ToArray()
                    ),
                ct
            );
    }
}

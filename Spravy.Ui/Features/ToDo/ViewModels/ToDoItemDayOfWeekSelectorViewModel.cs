namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfWeekSelectorViewModel : ViewModelBase, IApplySettings
{
    public ToDoItemDayOfWeekSelectorViewModel()
    {
        Items = new(Enum.GetValues<DayOfWeek>()
           .Select(x => new DayOfWeekSelectItem
            {
                DayOfWeek = x,
            }));

        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<DayOfWeekSelectItem> Items { get; }
    public ICommand InitializedCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Reactive]
    public Guid ToDoItemId { get; set; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.UpdateToDoItemWeeklyPeriodicityAsync(ToDoItemId,
            new(Items.Where(x => x.IsSelected).Select(x => x.DayOfWeek)), cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetWeeklyPeriodicityAsync(ToDoItemId, cancellationToken)
           .IfSuccessAsync(weeklyPeriodicity => Result.AwaitableFalse.IfSuccessAllAsync(cancellationToken,
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
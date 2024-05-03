namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfMonthSelectorViewModel : ViewModelBase, IApplySettings
{
    public ToDoItemDayOfMonthSelectorViewModel()
    {
        Items = new(Enumerable.Range(1, 31)
           .Select(x => new DayOfMonthSelectItem
            {
                Day = (byte)x,
            }));

        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<DayOfMonthSelectItem> Items { get; }
    public ICommand InitializedCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Reactive]
    public Guid ToDoItemId { get; set; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.UpdateToDoItemMonthlyPeriodicityAsync(ToDoItemId,
            new(Items.Where(x => x.IsSelected).Select(x => x.Day)), cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetMonthlyPeriodicityAsync(ToDoItemId, cancellationToken)
           .IfSuccessAsync(monthlyPeriodicity => Result.AwaitableFalse.IfSuccessAllAsync(cancellationToken,
                Items.Where(x => monthlyPeriodicity.Days.Contains(x.Day))
                   .Select<DayOfMonthSelectItem, Func<ConfiguredValueTaskAwaitable<Result>>>(x =>
                    {
                        var y = x;

                        return () => this.InvokeUIBackgroundAsync(() => y.IsSelected = true);
                    })
                   .ToArray()), cancellationToken);
    }
}
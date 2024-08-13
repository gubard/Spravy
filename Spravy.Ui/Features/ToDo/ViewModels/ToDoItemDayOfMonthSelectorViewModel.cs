namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfMonthSelectorViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;

    public ToDoItemDayOfMonthSelectorViewModel(ToDoItemEntityNotify item, IToDoService toDoService)
    {
        this.toDoService = toDoService;
        Item = item;
        DaysOfMonth = new();
        DaysOfMonth.AddRange(Item.DaysOfMonth);
    }

    public ToDoItemEntityNotify Item { get; }
    public AvaloniaList<int> DaysOfMonth { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.UpdateToDoItemMonthlyPeriodicityAsync(
            Item.Id,
            new(DaysOfMonth.Select(x => (byte)x).ToArray()),
            ct
        );
    }
}

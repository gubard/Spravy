namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfWeekSelectorViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;

    public ToDoItemDayOfWeekSelectorViewModel(ToDoItemEntityNotify item, IToDoService toDoService)
    {
        this.toDoService = toDoService;
        Item = item;
        DayOfWeeks = new(UiHelper.DayOfWeeks.Select(x => new Selected<DayOfWeek>(x)).ToArray());
        Update();
    }

    public AvaloniaList<Selected<DayOfWeek>> DayOfWeeks { get; }
    public ToDoItemEntityNotify Item { get; }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.UpdateToDoItemWeeklyPeriodicityAsync(
            Item.Id,
            new(DayOfWeeks.Where(x => x.IsSelect).Select(x => x.Value).ToArray()),
            ct
        );
    }

    private Result Update()
    {
        foreach (var dayOfWeek in DayOfWeeks)
        {
            if (Item.DaysOfWeek.Contains(dayOfWeek.Value))
            {
                dayOfWeek.IsSelect = true;
            }
        }

        return Result.Success;
    }

    public Result UpdateItemUi()
    {
        Item.DaysOfWeek.UpdateUi(DayOfWeeks.Where(x => x.IsSelect).Select(x => x.Value));

        return Result.Success;
    }
}

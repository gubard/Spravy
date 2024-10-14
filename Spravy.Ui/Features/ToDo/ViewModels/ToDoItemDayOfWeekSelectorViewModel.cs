namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfWeekSelectorViewModel : ViewModelBase, IEditToDoItems
{
    public ToDoItemDayOfWeekSelectorViewModel(ToDoItemEntityNotify item)
    {
        Item = item;
        DayOfWeeks = new(UiHelper.DayOfWeeks.Select(x => new Selected<DayOfWeek>(x)).ToArray());
        Update();
    }

    public AvaloniaList<Selected<DayOfWeek>> DayOfWeeks { get; }
    public ToDoItemEntityNotify Item { get; }

    public EditToDoItems GetEditToDoItems()
    {
        return new EditToDoItems()
            .SetIds(new[] { Item.Id })
            .SetWeeklyDays(new(DayOfWeeks.Where(x => x.IsSelect).Select(x => x.Value).ToArray()));
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

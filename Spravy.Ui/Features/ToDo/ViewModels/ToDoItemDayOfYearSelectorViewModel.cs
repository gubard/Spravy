namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfYearSelectorViewModel : ViewModelBase, IEditToDoItems
{
    public ToDoItemDayOfYearSelectorViewModel(ToDoItemEntityNotify item)
    {
        Item = item;

        DaysOfYear = new(
            Enumerable.Range(1, 12).Select(x => new DayOfYearSelectItem { Month = (byte)x })
        );

        var months = item.DaysOfYear.Select(x => x.Month).Distinct().ToArray();

        foreach (var daysOfYear in DaysOfYear)
        {
            if (months.Contains(daysOfYear.Month))
            {
                var days = item
                    .DaysOfYear.Where(x => x.Month == daysOfYear.Month)
                    .Select(x => x.Day)
                    .ToArray();

                foreach (var day in daysOfYear.Days.Where(x => days.Contains(x.Day)))
                {
                    day.IsSelected = true;
                }
            }
        }
    }

    public AvaloniaList<DayOfYearSelectItem> DaysOfYear { get; }
    public ToDoItemEntityNotify Item { get; }

    public EditToDoItems GetEditToDoItems()
    {
        return new EditToDoItems()
            .SetIds(new[] { Item.Id })
            .SetAnnuallyDays(
                new(
                    DaysOfYear
                        .SelectMany(x =>
                            x.Days.Where(y => y.IsSelected)
                                .Select(y => new DayOfYear(y.Day, x.Month))
                        )
                        .ToArray()
                )
            );
    }

    public Result UpdateItemUi()
    {
        Item.DaysOfYear.UpdateUi(
            DaysOfYear.SelectMany(x =>
                x.Days.Where(y => y.IsSelected).Select(y => new DayOfYear(y.Day, x.Month))
            )
        );

        return Result.Success;
    }
}

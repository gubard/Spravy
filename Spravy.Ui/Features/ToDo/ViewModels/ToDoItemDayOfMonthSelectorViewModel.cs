namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfMonthSelectorViewModel : ViewModelBase, IEditToDoItems
{
    public ToDoItemDayOfMonthSelectorViewModel(ToDoItemEntityNotify item)
    {
        Item = item;
        DaysOfMonth = new();
        DaysOfMonth.AddRange(Item.DaysOfMonth);
    }

    public ToDoItemEntityNotify Item { get; }
    public AvaloniaList<int> DaysOfMonth { get; }

    public EditToDoItems GetEditToDoItems()
    {
        return new EditToDoItems()
            .SetIds(new[] { Item.Id })
            .SetMonthlyDays(new(DaysOfMonth.Select(x => (byte)x).ToArray()));
    }

    public Result UpdateItemUi()
    {
        Item.DaysOfMonth.UpdateUi(DaysOfMonth);

        return Result.Success;
    }
}

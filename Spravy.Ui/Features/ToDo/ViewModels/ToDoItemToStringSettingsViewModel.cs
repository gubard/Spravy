namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemToStringSettingsViewModel : ViewModelBase
{
    private readonly AvaloniaList<CheckedItem<ToDoItemStatus>> statuses = new();

    public ToDoItemToStringSettingsViewModel(ToDoItemEntityNotify item)
    {
        Item = item;

        var items = UiHelper
            .ToDoItemStatuses.ToArray()
            .Select(x => new CheckedItem<ToDoItemStatus> { Item = x, IsChecked = true });

        statuses.AddRange(items);
    }

    public ToDoItemEntityNotify Item { get; }
    public IEnumerable<CheckedItem<ToDoItemStatus>> Statuses => statuses;
}

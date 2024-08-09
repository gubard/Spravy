namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemToStringSettingsViewModel : ViewModelBase
{
    public ToDoItemToStringSettingsViewModel()
    {
        var items = Enum.GetValuesAsUnderlyingType<ToDoItemStatus>()
            .OfType<ToDoItemStatus>()
            .Select(x => new CheckedItem<ToDoItemStatus> { Item = x, IsChecked = true, });

        Statuses.AddRange(items);
    }

    public AvaloniaList<CheckedItem<ToDoItemStatus>> Statuses { get; } = new();
}

using System.Linq;
using Avalonia.Collections;
using Spravy.ToDo.Domain.Enums;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemToStringSettingsViewModel : ViewModelBase
{
    public ToDoItemToStringSettingsViewModel()
    {
        var items = System.Enum.GetValues<ToDoItemStatus>()
            .Select(
                x => new CheckedItem<ToDoItemStatus>
                {
                    Item = x,
                    IsChecked = true
                }
            );

        Statuses.AddRange(items);
    }

    public AvaloniaList<CheckedItem<ToDoItemStatus>> Statuses { get; } = new();
}
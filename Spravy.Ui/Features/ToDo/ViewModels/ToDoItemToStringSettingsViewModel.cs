namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemToStringSettingsViewModel : DialogableViewModelBase
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

    public override string ViewId
    {
        get => $"{TypeCache<ToDoItemToStringSettingsViewModel>.Type}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}

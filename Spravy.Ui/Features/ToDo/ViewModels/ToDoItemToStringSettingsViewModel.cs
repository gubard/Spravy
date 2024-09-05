namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemToStringSettingsViewModel : DialogableViewModelBase
{
    private readonly AvaloniaList<CheckedItem<ToDoItemStatus>> statuses = new();

    public ToDoItemToStringSettingsViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        Item = item.GetNullable();
        Items = new(items.ToArray());

        var select = UiHelper
            .ToDoItemStatuses.ToArray()
            .Select(x => new CheckedItem<ToDoItemStatus> { Item = x, IsChecked = true });

        statuses.AddRange(select);
    }

    public ToDoItemEntityNotify? Item { get; }
    public AvaloniaList<ToDoItemEntityNotify> Items { get; }
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

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class RandomizeChildrenOrderViewModel : DialogableViewModelBase
{
    private readonly AvaloniaList<ToDoItemEntityNotify> items = new();

    public RandomizeChildrenOrderViewModel(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        this.items.AddRange(items.ToArray());
    }

    public RandomizeChildrenOrderViewModel(ToDoItemEntityNotify item)
    {
        Item = item;
    }

    public ToDoItemEntityNotify? Item { get; }
    public IEnumerable<ToDoItemEntityNotify> Items => items;

    public string Name
    {
        get => Item?.Name ?? Items.Select(x => x.Name).JoinString(", ");
    }

    public override string ViewId
    {
        get => $"{TypeCache<RandomizeChildrenOrderViewModel>.Type}";
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

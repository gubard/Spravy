namespace Spravy.Ui.Features.ToDo.ViewModels;

public class RandomizeChildrenOrderViewModel : ViewModelBase
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
}

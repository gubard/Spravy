namespace Spravy.Ui.Features.ToDo.ViewModels;

public class RandomizeChildrenOrderViewModel : ViewModelBase
{
    public ToDoItemEntityNotify? Item { get; set; }
    public ReadOnlyMemory<ToDoItemEntityNotify> Items { get; } = new();

    public string Name
    {
        get => Item?.Name ?? Items.Select(x => x.Name).JoinString(", ");
    }
}

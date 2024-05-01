namespace Spravy.Ui.Features.ToDo.ViewModels;

public class DependencyToDoItemsViewModel : ViewModelBase
{
    public DependencyToDoItemsViewModel()
    {
        DependencyToDoItems = new();
    }

    [Reactive]
    public Guid ToDoItemId { get; set; }

    public AvaloniaList<DependencyToDoItemNotify> DependencyToDoItems { get; }
}
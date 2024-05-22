namespace Spravy.Ui.Features.ToDo.ViewModels;

public class RandomizeChildrenOrderViewModel : ViewModelBase
{
    public ToDoItemEntityNotify? Item { get; set; }
    public ReadOnlyMemory<Guid> RandomizeChildrenOrderIds { get; set; } = ReadOnlyMemory<Guid>.Empty;
    
    [Inject]
    public required IToDoCache ToDoCache { get; init; }
    
    public string Name
    {
        get
        {
            if (RandomizeChildrenOrderIds.IsEmpty)
            {
                return Item?.Name ?? string.Empty;
            }
            
            return string.Join(", ",
                RandomizeChildrenOrderIds.Select(x => ToDoCache.GetToDoItem(x).ThrowIfError().Name).ToArray());
        }
    }
}
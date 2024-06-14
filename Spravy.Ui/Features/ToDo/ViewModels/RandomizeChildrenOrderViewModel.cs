namespace Spravy.Ui.Features.ToDo.ViewModels;

public class RandomizeChildrenOrderViewModel : ViewModelBase
{
    private readonly IToDoCache toDoCache;
    
    public RandomizeChildrenOrderViewModel(IToDoCache toDoCache)
    {
        this.toDoCache = toDoCache;
    }
    
    public ToDoItemEntityNotify? Item { get; set; }
    public ReadOnlyMemory<Guid> RandomizeChildrenOrderIds { get; set; } = ReadOnlyMemory<Guid>.Empty;
    
    public string Name
    {
        get
        {
            if (RandomizeChildrenOrderIds.IsEmpty)
            {
                return Item?.Name ?? string.Empty;
            }
            
            return string.Join(", ",
                RandomizeChildrenOrderIds.Select(x => toDoCache.GetToDoItem(x).ThrowIfError().Name).ToArray());
        }
    }
}
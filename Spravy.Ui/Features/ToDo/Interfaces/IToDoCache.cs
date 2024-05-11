namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoCache
{
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateRootItems(ReadOnlyMemory<Guid> roots);
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetRootItems();
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateChildrenItems(Guid id, ReadOnlyMemory<Guid> items);
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetChildrenItems(Guid id);
    Result<ToDoItemEntityNotify> GetToDoItem(Guid id);
    
    ConfiguredValueTaskAwaitable<Result<ToDoItemEntityNotify>> UpdateAsync(
        ToDoItem toDoItem,
        CancellationToken cancellationToken
    );
    
    ConfiguredValueTaskAwaitable<Result<ToDoItemEntityNotify>> UpdateAsync(
        ToDoShortItem toDoShortItem,
        CancellationToken cancellationToken
    );
    
    ConfiguredValueTaskAwaitable<Result> UpdateAsync(
        Guid id,
        ReadOnlyMemory<ToDoShortItem> parents,
        CancellationToken cancellationToken
    );
}
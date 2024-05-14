namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoCache
{
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateRootItems(ReadOnlyMemory<Guid> roots);
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetRootItems();
    Result<ToDoItemEntityNotify> GetToDoItem(Guid id);
    
    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoItemEntityNotify>>> UpdateChildrenItemsAsync(
        Guid id,
        ReadOnlyMemory<Guid> items,
        CancellationToken cancellationToken
    );
    
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
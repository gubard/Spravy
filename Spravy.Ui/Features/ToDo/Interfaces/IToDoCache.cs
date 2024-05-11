namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoCache
{
    ToDoShortItemNotify GetToDoShortItem(Guid id);
    ToDoSelectorItemNotify GetToDoSelectorItem(Guid id);
    ToDoItemParentNotify GetToDoItemParent(Guid id);
    Result<Selected<ToDoItemNotify>> GetToDoItem(Guid id);
    ActiveToDoItemNotify GetActiveToDoItem(Guid id);
    Result<ReadOnlyMemory<object>> GetToDoItemParents(Guid id);
    ConfiguredValueTaskAwaitable<Result> UpdateAsync(ToDoItem toDoItem, CancellationToken cancellationToken);
    
    ConfiguredValueTaskAwaitable<Result> UpdateParentsAsync(
        Guid id,
        ReadOnlyMemory<ToDoShortItem> parents,
        CancellationToken cancellationToken
    );
}
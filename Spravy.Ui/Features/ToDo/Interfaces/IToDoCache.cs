namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoCache
{
    Result<ToDoItemEntityNotify> GetToDoItem(Guid id);
    ConfiguredValueTaskAwaitable<Result<ToDoItemEntityNotify>> UpdateAsync(ToDoItem toDoItem,  CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result<ToDoItemEntityNotify>> UpdateAsync(ToDoShortItem toDoShortItem,  CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result> UpdateAsync(Guid id, ReadOnlyMemory<ToDoShortItem> parents, CancellationToken cancellationToken);
}
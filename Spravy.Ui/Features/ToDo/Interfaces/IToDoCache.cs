namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoCache
{
    Result ResetItemsUi();
    Result IgnoreItemsUi(ReadOnlyMemory<Guid> ids);
    Result ExpandItemUi(Guid id);
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateRootItems(ReadOnlyMemory<Guid> roots);
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetRootItems();
    Result<ToDoItemEntityNotify> GetToDoItem(Guid id);
    Result<ToDoItemEntityNotify> UpdateUi(FullToDoItem toDoItem);
    Result UpdateParentsUi(Guid id, ReadOnlyMemory<ToDoShortItem> parents);
    Result<ToDoItemEntityNotify> UpdateUi(ToDoShortItem toDoShortItem);
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateUi(ReadOnlyMemory<ToDoSelectorItem> items);
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateChildrenItemsUi(Guid id, ReadOnlyMemory<Guid> items);
}
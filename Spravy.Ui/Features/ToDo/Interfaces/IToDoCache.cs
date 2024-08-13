namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoCache
{
    Result ResetItemsUi();
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateRootItems(ReadOnlyMemory<Guid> roots);
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetRootItems();
    Result<ToDoItemEntityNotify> GetToDoItem(Guid id);
    Result<ToDoItemEntityNotify> UpdateUi(ToDoItem toDoItem);
    Result<ToDoItemEntityNotify> UpdateUi(FullToDoItem toDoItem);
    Result<ToDoItemEntityNotify> UpdateUi(Guid id, PeriodicityOffsetToDoItemSettings settings);
    Result<ToDoItemEntityNotify> UpdateUi(Guid id, PeriodicityToDoItemSettings settings);
    Result<ToDoItemEntityNotify> UpdateUi(Guid id, PlannedToDoItemSettings settings);
    Result<ToDoItemEntityNotify> UpdateUi(Guid id, ValueToDoItemSettings settings);
    Result<ToDoItemEntityNotify> UpdateUi(Guid id, WeeklyPeriodicity periodicity);
    Result<ToDoItemEntityNotify> UpdateUi(Guid id, MonthlyPeriodicity periodicity);
    Result<ToDoItemEntityNotify> UpdateUi(Guid id, AnnuallyPeriodicity periodicity);
    Result<ToDoItemEntityNotify> UpdateUi(ActiveToDoItem activeToDoItem);
    Result UpdateParentsUi(Guid id, ReadOnlyMemory<ToDoShortItem> parents);
    Result<ToDoItemEntityNotify> UpdateUi(ToDoShortItem toDoShortItem);
    Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateUi(ReadOnlyMemory<ToDoSelectorItem> items);

    Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateChildrenItemsUi(
        Guid id,
        ReadOnlyMemory<Guid> items
    );
}

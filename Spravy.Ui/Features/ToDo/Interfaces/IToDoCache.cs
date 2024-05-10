namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoCache
{
    ToDoItemNotify GetToDoItem(Guid id);
    ActiveToDoItemNotify GetActiveToDoItem(Guid id);
    void Update(ToDoItem toDoItem);
}
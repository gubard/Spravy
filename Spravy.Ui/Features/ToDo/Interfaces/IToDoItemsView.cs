namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoItemsView
{
    Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items);
    Result AddOrUpdateUi(ToDoItemEntityNotify item);
}

namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoItemsView : IRefresh
{
    Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items);
    Result AddOrUpdateUi(ToDoItemEntityNotify item);
    Result RemoveUi(ToDoItemEntityNotify item);
}

namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoItemsView : IRefresh, IRemove
{
    Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items);
    Result AddOrUpdateUi(ToDoItemEntityNotify item);
}

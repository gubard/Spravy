namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoItemsView : IRefresh, IRemove
{
    Result SetItemsUi(ReadOnlyMemory<ToDoItemEntityNotify> items);
    Result AddOrUpdateUi(ReadOnlyMemory<ToDoItemEntityNotify> items);
}

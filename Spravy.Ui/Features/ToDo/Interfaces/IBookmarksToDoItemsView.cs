namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IBookmarksToDoItemsView
{
    Result SetBookmarksUi(ReadOnlyMemory<ToDoItemEntityNotify> items);
}
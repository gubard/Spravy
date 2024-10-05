namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IBookmarksToDoItemsView
{
    Result ClearBookmarksExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items);
}
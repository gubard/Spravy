namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoUiService
{
    Cvtar UpdateItemAsync(ToDoItemEntityNotify item, CancellationToken ct);
    Cvtar UpdateItemsAsync(ReadOnlyMemory<ToDoItemEntityNotify> items, CancellationToken ct);
    Cvtar UpdateRootItemsAsync(IToDoItemsView toDoItemsView, CancellationToken ct);
    Cvtar UpdateTodayItemsAsync(IToDoItemsView toDoItemsView, CancellationToken ct);

    Cvtar UpdateBookmarkItemsAsync(
        IBookmarksToDoItemsView bookmarksToDoItemsView,
        CancellationToken ct
    );

    Cvtar UpdateSelectorItemsAsync(
        Guid? selectedId,
        ReadOnlyMemory<Guid> ignoreIds,
        CancellationToken ct
    );

    Cvtar UpdateSiblingsAsync(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> ignoreItems,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    );

    Cvtar UpdateLeafToDoItemsAsync(
        ToDoItemEntityNotify item,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    );

    Cvtar UpdateSearchToDoItemsAsync(
        string searchText,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    );

    Cvtar UpdateItemChildrenAsync(
        ToDoItemEntityNotify item,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    );
}

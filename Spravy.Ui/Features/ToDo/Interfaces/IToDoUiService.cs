namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoUiService
{
    ConfiguredValueTaskAwaitable<Result> UpdateItemAsync(
        ToDoItemEntityNotify item,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result> UpdateItemsAsync(
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoItemEntityNotify>>> UpdateSiblingsAsync(
        ToDoItemEntityNotify item,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result> UpdateLeafToDoItemsAsync(
        ToDoItemEntityNotify item,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result> UpdateRootItemsAsync(
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result> UpdateSearchToDoItemsAsync(
        string searchText,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result> UpdateItemChildrenAsync(
        ToDoItemEntityNotify item,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result> UpdateTodayItemsAsync(
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    );
}

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
}

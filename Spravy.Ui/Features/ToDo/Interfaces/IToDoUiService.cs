namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoUiService
{
    ConfiguredValueTaskAwaitable<Result> UpdateItemsAsync(
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoItemEntityNotify>>> GetSiblingsAsync(
        ToDoItemEntityNotify item,
        CancellationToken ct
    );
}

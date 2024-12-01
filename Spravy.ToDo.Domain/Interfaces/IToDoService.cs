using System.Runtime.CompilerServices;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Domain.Interfaces;

public interface IToDoService
{
    Cvtar DeleteToDoItemsAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct);
    Cvtar RandomizeChildrenOrderIndexAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct);
    Cvtar ResetToDoItemAsync(ReadOnlyMemory<ResetToDoItemOptions> options, CancellationToken ct);
    Cvtar EditToDoItemsAsync(EditToDoItems options, CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result<bool>> UpdateEventsAsync(CancellationToken ct);
    Cvtar SwitchCompleteAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetShortToDoItemsAsync(
        ReadOnlyMemory<Guid> ids,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<OptionStruct<ToDoShortItem>>> GetCurrentActiveToDoItemAsync(
        CancellationToken ct
    );

    Cvtar UpdateToDoItemOrderIndexAsync(ReadOnlyMemory<UpdateOrderIndexToDoItemOptions> options, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetBookmarkToDoItemIdsAsync(CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<FullToDoItem>> GetToDoItemAsync(Guid id, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<OptionStruct<ToDoShortItem>>> GetActiveToDoItemAsync(
        Guid id,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(Guid id, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsAsync(
        string searchText,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(Guid id, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(
        OptionStruct<Guid> id,
        ReadOnlyMemory<Guid> ignoreIds,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> AddToDoItemAsync(
        ReadOnlyMemory<AddToDoItemOptions> options,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<string>> ToDoItemToStringAsync(
        ReadOnlyMemory<ToDoItemToStringOptions> options,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> CloneToDoItemAsync(
        ReadOnlyMemory<Guid> cloneIds,
        OptionStruct<Guid> parentId,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoSelectorItem>>> GetToDoSelectorItemsAsync(
        ReadOnlyMemory<Guid> ignoreIds,
        CancellationToken ct
    );

    ConfiguredCancelableAsyncEnumerable<Result<ReadOnlyMemory<FullToDoItem>>> GetToDoItemsAsync(
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        CancellationToken ct
    );
}
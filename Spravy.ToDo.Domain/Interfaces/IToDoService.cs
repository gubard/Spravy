using System.Runtime.CompilerServices;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Domain.Interfaces;

public interface IToDoService
{
    ConfiguredValueTaskAwaitable<Result<ToDoResponse>> GetAsync(GetToDo get, CancellationToken ct);
    Cvtar DeleteToDoItemsAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct);
    Cvtar RandomizeChildrenOrderIndexAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct);
    Cvtar ResetToDoItemAsync(ReadOnlyMemory<ResetToDoItemOptions> options, CancellationToken ct);
    Cvtar EditToDoItemsAsync(EditToDoItems options, CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result<bool>> UpdateEventsAsync(CancellationToken ct);
    Cvtar SwitchCompleteAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct);
    Cvtar UpdateToDoItemOrderIndexAsync(ReadOnlyMemory<UpdateOrderIndexToDoItemOptions> options, CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> AddToDoItemAsync(ReadOnlyMemory<AddToDoItemOptions> options, CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> CloneToDoItemAsync(ReadOnlyMemory<Guid> cloneIds, OptionStruct<Guid> parentId, CancellationToken ct);
}
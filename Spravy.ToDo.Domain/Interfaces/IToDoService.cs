using System.Runtime.CompilerServices;
using Spravy.Domain.Enums;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Domain.Interfaces;

public interface IToDoService
{
    Cvtar DeleteToDoItemAsync(Guid id, CancellationToken ct);
    Cvtar AddFavoriteToDoItemAsync(Guid id, CancellationToken ct);
    Cvtar RemoveFavoriteToDoItemAsync(Guid id, CancellationToken ct);
    Cvtar ToDoItemToRootAsync(Guid id, CancellationToken ct);
    Cvtar UpdateIsBookmarkAsync(Guid id, bool isBookmark, CancellationToken ct);
    Cvtar RandomizeChildrenOrderIndexAsync(Guid id, CancellationToken ct);
    Cvtar UpdateReferenceToDoItemAsync(Guid id, Guid referenceId, CancellationToken ct);
    Cvtar ResetToDoItemAsync(ResetToDoItemOptions options, CancellationToken ct);
    Cvtar UpdateToDoItemDescriptionTypeAsync(Guid id, DescriptionType type, CancellationToken ct);
    Cvtar UpdateToDoItemDueDateAsync(Guid id, DateOnly dueDate, CancellationToken ct);
    Cvtar UpdateToDoItemNameAsync(Guid id, string name, CancellationToken ct);
    Cvtar UpdateToDoItemDescriptionAsync(Guid id, string description, CancellationToken ct);
    Cvtar UpdateToDoItemCompleteStatusAsync(Guid id, bool isComplete, CancellationToken ct);
    Cvtar UpdateToDoItemTypeAsync(Guid id, ToDoItemType type, CancellationToken ct);
    Cvtar UpdateToDoItemParentAsync(Guid id, Guid parentId, CancellationToken ct);
    Cvtar UpdateToDoItemDaysOffsetAsync(Guid id, ushort days, CancellationToken ct);
    Cvtar UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months, CancellationToken ct);
    Cvtar UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks, CancellationToken ct);
    Cvtar UpdateToDoItemYearsOffsetAsync(Guid id, ushort years, CancellationToken ct);
    Cvtar UpdateToDoItemLinkAsync(Guid id, Option<Uri> link, CancellationToken ct);
    Cvtar UpdateToDoItemChildrenTypeAsync(Guid id, ToDoItemChildrenType type, CancellationToken ct);
    Cvtar UpdateToDoItemIsRequiredCompleteInDueDateAsync(Guid id, bool value, CancellationToken ct);
    Cvtar UpdateIconAsync(Guid id, string icon, CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result<bool>> UpdateEventsAsync(CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetBookmarkToDoItemIdsAsync(
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<FullToDoItem>> GetToDoItemAsync(
        Guid id,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<OptionStruct<ActiveToDoItem>>> GetActiveToDoItemAsync(
        Guid id,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(
        Guid id,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsAsync(
        string searchText,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(
        Guid id,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(
        Guid id,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetRootToDoItemIdsAsync(
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<Guid>> AddToDoItemAsync(
        AddToDoItemOptions options,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<string>> ToDoItemToStringAsync(
        ToDoItemToStringOptions options,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetSiblingsAsync(
        Guid id,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<
        Result<OptionStruct<ActiveToDoItem>>
    > GetCurrentActiveToDoItemAsync(CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<PlannedToDoItemSettings>> GetPlannedToDoItemSettingsAsync(
        Guid id,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ValueToDoItemSettings>> GetValueToDoItemSettingsAsync(
        Guid id,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<WeeklyPeriodicity>> GetWeeklyPeriodicityAsync(
        Guid id,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<MonthlyPeriodicity>> GetMonthlyPeriodicityAsync(
        Guid id,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<AnnuallyPeriodicity>> GetAnnuallyPeriodicityAsync(
        Guid id,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<Guid>> CloneToDoItemAsync(
        Guid cloneId,
        OptionStruct<Guid> parentId,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<ToDoShortItem>>
    > GetChildrenToDoItemShortsAsync(Guid id, CancellationToken ct);

    ConfiguredValueTaskAwaitable<
        Result<PeriodicityToDoItemSettings>
    > GetPeriodicityToDoItemSettingsAsync(Guid id, CancellationToken ct);

    Cvtar UpdateToDoItemTypeOfPeriodicityAsync(
        Guid id,
        TypeOfPeriodicity type,
        CancellationToken ct
    );

    Cvtar UpdateToDoItemOrderIndexAsync(
        UpdateOrderIndexToDoItemOptions options,
        CancellationToken ct
    );

    Cvtar UpdateToDoItemAnnuallyPeriodicityAsync(
        Guid id,
        AnnuallyPeriodicity periodicity,
        CancellationToken ct
    );

    Cvtar UpdateToDoItemMonthlyPeriodicityAsync(
        Guid id,
        MonthlyPeriodicity periodicity,
        CancellationToken ct
    );

    Cvtar UpdateToDoItemWeeklyPeriodicityAsync(
        Guid id,
        WeeklyPeriodicity periodicity,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<ToDoSelectorItem>>
    > GetToDoSelectorItemsAsync(ReadOnlyMemory<Guid> ignoreIds, CancellationToken ct);

    ConfiguredValueTaskAwaitable<
        Result<PeriodicityOffsetToDoItemSettings>
    > GetPeriodicityOffsetToDoItemSettingsAsync(Guid id, CancellationToken ct);

    ConfiguredCancelableAsyncEnumerable<Result<ReadOnlyMemory<ToDoItem>>> GetToDoItemsAsync(
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        CancellationToken ct
    );
}

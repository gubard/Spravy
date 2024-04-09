using System.Runtime.CompilerServices;
using Spravy.Domain.Enums;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Domain.Interfaces;

public interface IToDoService
{
    ConfiguredValueTaskAwaitable<Result> ResetToDoItemAsync(Guid id, CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result> RandomizeChildrenOrderIndexAsync(Guid id, CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result> DeleteToDoItemAsync(Guid id, CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result> AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result> RemoveFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result> ToDoItemToRootAsync(Guid id, CancellationToken cancellationToken);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsAsync(
        string searchText,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ToDoItem>> GetToDoItemAsync(Guid id, CancellationToken cancellationToken);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetRootToDoItemIdsAsync(
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<Guid>> AddRootToDoItemAsync(
        AddRootToDoItemOptions options,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<Guid>> AddToDoItemAsync(
        AddToDoItemOptions options,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<string>> ToDoItemToStringAsync(
        ToDoItemToStringOptions options,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetSiblingsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ActiveToDoItem?>> GetCurrentActiveToDoItemAsync(
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<PlannedToDoItemSettings>> GetPlannedToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ValueToDoItemSettings>> GetValueToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<WeeklyPeriodicity>> GetWeeklyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<MonthlyPeriodicity>> GetMonthlyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<AnnuallyPeriodicity>> GetAnnuallyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> CloneToDoItemAsync(
        Guid cloneId,
        Guid? parentId,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDescriptionTypeAsync(
        Guid id,
        DescriptionType type,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDueDateAsync(
        Guid id,
        DateOnly dueDate,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemNameAsync(
        Guid id,
        string name,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDescriptionAsync(
        Guid id,
        string description,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemCompleteStatusAsync(
        Guid id,
        bool isComplete,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemTypeAsync(
        Guid id,
        ToDoItemType type,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemParentAsync(
        Guid id,
        Guid parentId,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDaysOffsetAsync(
        Guid id,
        ushort days,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemMonthsOffsetAsync(
        Guid id,
        ushort months,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemWeeksOffsetAsync(
        Guid id,
        ushort weeks,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemYearsOffsetAsync(
        Guid id,
        ushort years,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemLinkAsync(
        Guid id,
        Uri? link,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetChildrenToDoItemShortsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<PeriodicityToDoItemSettings>> GetPeriodicityToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemTypeOfPeriodicityAsync(
        Guid id,
        TypeOfPeriodicity type,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemOrderIndexAsync(
        UpdateOrderIndexToDoItemOptions options,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemChildrenTypeAsync(
        Guid id,
        ToDoItemChildrenType type,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemIsRequiredCompleteInDueDateAsync(
        Guid id,
        bool value,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemAnnuallyPeriodicityAsync(
        Guid id,
        AnnuallyPeriodicity periodicity,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemMonthlyPeriodicityAsync(
        Guid id,
        MonthlyPeriodicity periodicity,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateToDoItemWeeklyPeriodicityAsync(
        Guid id,
        WeeklyPeriodicity periodicity,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoSelectorItem>>> GetToDoSelectorItemsAsync(
        Guid[] ignoreIds,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<PeriodicityOffsetToDoItemSettings>> GetPeriodicityOffsetToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    IAsyncEnumerable<ReadOnlyMemory<ToDoItem>> GetToDoItemsAsync(
        Guid[] ids,
        uint chunkSize,
        CancellationToken cancellationToken
    );
}
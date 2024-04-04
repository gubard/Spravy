using Spravy.Domain.Enums;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Domain.Interfaces;

public interface IToDoService
{
    ValueTask<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsAsync(string searchText, CancellationToken cancellationToken);
    ValueTask<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result<ToDoItem>> GetToDoItemAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result<ReadOnlyMemory<Guid>>> GetRootToDoItemIdsAsync(CancellationToken cancellationToken);
    ValueTask<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(CancellationToken cancellationToken);
    ValueTask<Result<Guid>> AddRootToDoItemAsync(AddRootToDoItemOptions options, CancellationToken cancellationToken);
    ValueTask<Result<Guid>> AddToDoItemAsync(AddToDoItemOptions options, CancellationToken cancellationToken);
    ValueTask<Result<string>> ToDoItemToStringAsync(ToDoItemToStringOptions options, CancellationToken cancellationToken);
    ValueTask<Result<ReadOnlyMemory<ToDoShortItem>>> GetSiblingsAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result<ActiveToDoItem?>> GetCurrentActiveToDoItemAsync(CancellationToken cancellationToken);
    ValueTask<Result<PlannedToDoItemSettings>> GetPlannedToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result<ValueToDoItemSettings>> GetValueToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result<WeeklyPeriodicity>> GetWeeklyPeriodicityAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result<MonthlyPeriodicity>> GetMonthlyPeriodicityAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result<AnnuallyPeriodicity>> GetAnnuallyPeriodicityAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(CancellationToken cancellationToken);
    ValueTask<Result> CloneToDoItemAsync(Guid cloneId, Guid? parentId, CancellationToken cancellationToken);
    ValueTask<Result> UpdateToDoItemDescriptionTypeAsync(Guid id, DescriptionType type, CancellationToken cancellationToken);
    ValueTask<Result> ResetToDoItemAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result> RandomizeChildrenOrderIndexAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result> DeleteToDoItemAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result> UpdateToDoItemDueDateAsync(Guid id, DateOnly dueDate, CancellationToken cancellationToken);
    ValueTask<Result> UpdateToDoItemNameAsync(Guid id, string name, CancellationToken cancellationToken);
    ValueTask<Result> UpdateToDoItemDescriptionAsync(Guid id, string description, CancellationToken cancellationToken);
    ValueTask<Result> UpdateToDoItemCompleteStatusAsync(Guid id, bool isComplete, CancellationToken cancellationToken);
    ValueTask<Result> UpdateToDoItemTypeAsync(Guid id, ToDoItemType type, CancellationToken cancellationToken);
    ValueTask<Result> AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result> RemoveFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result> UpdateToDoItemParentAsync(Guid id, Guid parentId, CancellationToken cancellationToken);
    ValueTask<Result> ToDoItemToRootAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<Result> UpdateToDoItemDaysOffsetAsync(Guid id, ushort days, CancellationToken cancellationToken);
    ValueTask<Result> UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months, CancellationToken cancellationToken);
    ValueTask<Result> UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks, CancellationToken cancellationToken);
    ValueTask<Result> UpdateToDoItemYearsOffsetAsync(Guid id, ushort years, CancellationToken cancellationToken);
    ValueTask<Result> UpdateToDoItemLinkAsync(Guid id, Uri? link, CancellationToken cancellationToken);

    ValueTask<Result<ReadOnlyMemory<ToDoShortItem>>> GetChildrenToDoItemShortsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ValueTask<Result<PeriodicityToDoItemSettings>> GetPeriodicityToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    ValueTask<Result> UpdateToDoItemTypeOfPeriodicityAsync(
        Guid id,
        TypeOfPeriodicity type,
        CancellationToken cancellationToken
    );

    ValueTask<Result> UpdateToDoItemOrderIndexAsync(
        UpdateOrderIndexToDoItemOptions options,
        CancellationToken cancellationToken
    );

    ValueTask<Result> UpdateToDoItemChildrenTypeAsync(
        Guid id,
        ToDoItemChildrenType type,
        CancellationToken cancellationToken
    );

    ValueTask<Result> UpdateToDoItemIsRequiredCompleteInDueDateAsync(
        Guid id,
        bool value,
        CancellationToken cancellationToken
    );

    ValueTask<Result> UpdateToDoItemAnnuallyPeriodicityAsync(
        Guid id,
        AnnuallyPeriodicity periodicity,
        CancellationToken cancellationToken
    );

    ValueTask<Result> UpdateToDoItemMonthlyPeriodicityAsync(
        Guid id,
        MonthlyPeriodicity periodicity,
        CancellationToken cancellationToken
    );

    ValueTask<Result> UpdateToDoItemWeeklyPeriodicityAsync(
        Guid id,
        WeeklyPeriodicity periodicity,
        CancellationToken cancellationToken
    );

    ValueTask<Result<ReadOnlyMemory<ToDoSelectorItem>>> GetToDoSelectorItemsAsync(
        Guid[] ignoreIds,
        CancellationToken cancellationToken
    );

    ValueTask<Result<PeriodicityOffsetToDoItemSettings>> GetPeriodicityOffsetToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    IAsyncEnumerable<ReadOnlyMemory<ToDoItem>> GetToDoItemsAsync(
        Guid[] ids,
        uint chunkSize,
        CancellationToken cancellationToken
    );
}
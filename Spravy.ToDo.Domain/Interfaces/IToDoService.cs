using Spravy.Domain.Enums;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Domain.Interfaces;

public interface IToDoService
{
    Task<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsAsync(string searchText, CancellationToken cancellationToken);
    Task<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<ToDoItem>> GetToDoItemAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<ReadOnlyMemory<Guid>>> GetRootToDoItemIdsAsync(CancellationToken cancellationToken);
    Task<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(CancellationToken cancellationToken);
    Task<Result<Guid>> AddRootToDoItemAsync(AddRootToDoItemOptions options, CancellationToken cancellationToken);
    Task<Result<Guid>> AddToDoItemAsync(AddToDoItemOptions options, CancellationToken cancellationToken);
    Task<Result<string>> ToDoItemToStringAsync(ToDoItemToStringOptions options, CancellationToken cancellationToken);
    Task<Result<ReadOnlyMemory<ToDoShortItem>>> GetSiblingsAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<ActiveToDoItem?>> GetCurrentActiveToDoItemAsync(CancellationToken cancellationToken);
    Task<Result<PlannedToDoItemSettings>> GetPlannedToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<ValueToDoItemSettings>> GetValueToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<WeeklyPeriodicity>> GetWeeklyPeriodicityAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<MonthlyPeriodicity>> GetMonthlyPeriodicityAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<AnnuallyPeriodicity>> GetAnnuallyPeriodicityAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(CancellationToken cancellationToken);
    Task<Result> CloneToDoItemAsync(Guid cloneId, Guid? parentId, CancellationToken cancellationToken);
    Task<Result> UpdateToDoItemDescriptionTypeAsync(Guid id, DescriptionType type, CancellationToken cancellationToken);
    Task<Result> ResetToDoItemAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> RandomizeChildrenOrderIndexAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> DeleteToDoItemAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> UpdateToDoItemDueDateAsync(Guid id, DateOnly dueDate, CancellationToken cancellationToken);
    Task<Result> UpdateToDoItemNameAsync(Guid id, string name, CancellationToken cancellationToken);
    Task<Result> UpdateToDoItemDescriptionAsync(Guid id, string description, CancellationToken cancellationToken);
    Task<Result> UpdateToDoItemCompleteStatusAsync(Guid id, bool isComplete, CancellationToken cancellationToken);
    Task<Result> UpdateToDoItemTypeAsync(Guid id, ToDoItemType type, CancellationToken cancellationToken);
    Task<Result> AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> RemoveFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> UpdateToDoItemParentAsync(Guid id, Guid parentId, CancellationToken cancellationToken);
    Task<Result> ToDoItemToRootAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> UpdateToDoItemDaysOffsetAsync(Guid id, ushort days, CancellationToken cancellationToken);
    Task<Result> UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months, CancellationToken cancellationToken);
    Task<Result> UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks, CancellationToken cancellationToken);
    Task<Result> UpdateToDoItemYearsOffsetAsync(Guid id, ushort years, CancellationToken cancellationToken);
    Task<Result> UpdateToDoItemLinkAsync(Guid id, Uri? link, CancellationToken cancellationToken);

    Task<Result<ReadOnlyMemory<ToDoShortItem>>> GetChildrenToDoItemShortsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    Task<Result<PeriodicityToDoItemSettings>> GetPeriodicityToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    Task<Result> UpdateToDoItemTypeOfPeriodicityAsync(
        Guid id,
        TypeOfPeriodicity type,
        CancellationToken cancellationToken
    );

    Task<Result> UpdateToDoItemOrderIndexAsync(
        UpdateOrderIndexToDoItemOptions options,
        CancellationToken cancellationToken
    );

    Task<Result> UpdateToDoItemChildrenTypeAsync(
        Guid id,
        ToDoItemChildrenType type,
        CancellationToken cancellationToken
    );

    Task<Result> UpdateToDoItemIsRequiredCompleteInDueDateAsync(
        Guid id,
        bool value,
        CancellationToken cancellationToken
    );

    Task<Result> UpdateToDoItemAnnuallyPeriodicityAsync(
        Guid id,
        AnnuallyPeriodicity periodicity,
        CancellationToken cancellationToken
    );

    Task<Result> UpdateToDoItemMonthlyPeriodicityAsync(
        Guid id,
        MonthlyPeriodicity periodicity,
        CancellationToken cancellationToken
    );

    Task<Result> UpdateToDoItemWeeklyPeriodicityAsync(
        Guid id,
        WeeklyPeriodicity periodicity,
        CancellationToken cancellationToken
    );

    Task<Result<ReadOnlyMemory<ToDoSelectorItem>>> GetToDoSelectorItemsAsync(
        Guid[] ignoreIds,
        CancellationToken cancellationToken
    );

    Task<Result<PeriodicityOffsetToDoItemSettings>> GetPeriodicityOffsetToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    IAsyncEnumerable<ReadOnlyMemory<ToDoItem>> GetToDoItemsAsync(
        Guid[] ids,
        uint chunkSize,
        CancellationToken cancellationToken
    );
}
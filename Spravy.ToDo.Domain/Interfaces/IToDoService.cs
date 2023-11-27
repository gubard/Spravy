using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Domain.Interfaces;

public interface IToDoService
{
    Task<IEnumerable<ToDoShortItem>> GetParentsAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> SearchToDoItemIdsAsync(string searchText, CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> GetLeafToDoItemIdsAsync(Guid id, CancellationToken cancellationToken);
    Task<ToDoItem> GetToDoItemAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> GetChildrenToDoItemIdsAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> GetRootToDoItemIdsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> GetFavoriteToDoItemIdsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<IToDoSubItem>> GetRootToDoSubItemsAsync(CancellationToken cancellationToken);
    Task<IToDoItem> GetToDoItem2Async(Guid id, CancellationToken cancellationToken);
    Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options, CancellationToken cancellationToken);
    Task<Guid> AddToDoItemAsync(AddToDoItemOptions options, CancellationToken cancellationToken);
    Task DeleteToDoItemAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateToDoItemTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type, CancellationToken cancellationToken);
    Task UpdateToDoItemDueDateAsync(Guid id, DateOnly dueDate, CancellationToken cancellationToken);
    Task UpdateToDoItemNameAsync(Guid id, string name, CancellationToken cancellationToken);
    Task UpdateToDoItemOrderIndexAsync(UpdateOrderIndexToDoItemOptions options, CancellationToken cancellationToken);
    Task UpdateToDoItemDescriptionAsync(Guid id, string description, CancellationToken cancellationToken);
    Task UpdateToDoItemCompleteStatusAsync(Guid id, bool isComplete, CancellationToken cancellationToken);
    Task SkipToDoItemAsync(Guid id, CancellationToken cancellationToken);
    Task FailToDoItemAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<IToDoSubItem>> SearchToDoSubItemsAsync(string searchText, CancellationToken cancellationToken);
    Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type, CancellationToken cancellationToken);
    Task AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken);
    Task RemoveFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<IToDoSubItem>> GetFavoriteToDoItemsAsync( CancellationToken cancellationToken);
    Task UpdateToDoItemAnnuallyPeriodicityAsync(Guid id, AnnuallyPeriodicity periodicity, CancellationToken cancellationToken);
    Task UpdateToDoItemMonthlyPeriodicityAsync(Guid id, MonthlyPeriodicity periodicity, CancellationToken cancellationToken);
    Task UpdateToDoItemWeeklyPeriodicityAsync(Guid id, WeeklyPeriodicity periodicity, CancellationToken cancellationToken);
    Task<IEnumerable<IToDoSubItem>> GetLeafToDoSubItemsAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<ToDoSelectorItem>> GetToDoSelectorItemsAsync(Guid[] ignoreIds, CancellationToken cancellationToken);
    Task UpdateToDoItemParentAsync(Guid id, Guid parentId, CancellationToken cancellationToken);
    Task ToDoItemToRootAsync(Guid id, CancellationToken cancellationToken);
    Task<string> ToDoItemToStringAsync(ToDoItemToStringOptions options, CancellationToken cancellationToken);
    Task UpdateToDoItemDaysOffsetAsync(Guid id, ushort days, CancellationToken cancellationToken);
    Task UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months, CancellationToken cancellationToken);
    Task UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks, CancellationToken cancellationToken);
    Task UpdateToDoItemYearsOffsetAsync(Guid id, ushort years, CancellationToken cancellationToken);
    Task UpdateToDoItemChildrenTypeAsync(Guid id, ToDoItemChildrenType type, CancellationToken cancellationToken);
    Task<IEnumerable<ToDoShortItem>> GetSiblingsAsync(Guid id, CancellationToken cancellationToken);
    Task<ActiveToDoItem?> GetCurrentActiveToDoItemAsync(CancellationToken cancellationToken);
    Task UpdateToDoItemLinkAsync(Guid id, Uri? link, CancellationToken cancellationToken);
    Task<PlannedToDoItemSettings> GetPlannedToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken);
    Task<ValueToDoItemSettings> GetValueToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken);
    Task<PeriodicityToDoItemSettings> GetPeriodicityToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken);
    Task<WeeklyPeriodicity> GetWeeklyPeriodicityAsync(Guid id, CancellationToken cancellationToken);
    Task<MonthlyPeriodicity> GetMonthlyPeriodicityAsync(Guid id, CancellationToken cancellationToken);
    Task<AnnuallyPeriodicity> GetAnnuallyPeriodicityAsync(Guid id, CancellationToken cancellationToken);
    Task<PeriodicityOffsetToDoItemSettings> GetPeriodicityOffsetToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken);
}
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Domain.Interfaces;

public interface IToDoService
{
    Task<IEnumerable<IToDoSubItem>> GetRootToDoSubItemsAsync(TimeSpan offset);
    Task<IToDoItem> GetToDoItemAsync(Guid id, TimeSpan offset);
    Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options);
    Task<Guid> AddToDoItemAsync(AddToDoItemOptions options, TimeSpan offset);
    Task DeleteToDoItemAsync(Guid id);
    Task UpdateToDoItemTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type);
    Task UpdateToDoItemDueDateAsync(Guid id, DateOnly dueDate);
    Task UpdateToDoItemNameAsync(Guid id, string name);
    Task UpdateToDoItemOrderIndexAsync(UpdateOrderIndexToDoItemOptions options);
    Task UpdateToDoItemDescriptionAsync(Guid id, string description);
    Task UpdateToDoItemCompleteStatusAsync(Guid id, bool isCompleted, TimeSpan offset);
    Task SkipToDoItemAsync(Guid id, TimeSpan offset);
    Task FailToDoItemAsync(Guid id, TimeSpan offset);
    Task<IEnumerable<IToDoSubItem>> SearchToDoSubItemsAsync(string searchText, TimeSpan offset);
    Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type);
    Task AddFavoriteToDoItemAsync(Guid id);
    Task RemoveFavoriteToDoItemAsync(Guid id);
    Task<IEnumerable<IToDoSubItem>> GetFavoriteToDoItemsAsync( TimeSpan offset);
    Task UpdateToDoItemAnnuallyPeriodicityAsync(Guid id, AnnuallyPeriodicity periodicity);
    Task UpdateToDoItemMonthlyPeriodicityAsync(Guid id, MonthlyPeriodicity periodicity);
    Task UpdateToDoItemWeeklyPeriodicityAsync(Guid id, WeeklyPeriodicity periodicity);
    Task<IEnumerable<IToDoSubItem>> GetLeafToDoSubItemsAsync(Guid id, TimeSpan offset);
    Task<IEnumerable<ToDoSelectorItem>> GetToDoSelectorItemsAsync(Guid[] ignoreIds);
    Task UpdateToDoItemParentAsync(Guid id, Guid parentId);
    Task ToDoItemToRootAsync(Guid id);
    Task<string> ToDoItemToStringAsync(ToDoItemToStringOptions options, TimeSpan offset);
    Task UpdateToDoItemDaysOffsetAsync(Guid id, ushort days);
    Task UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months);
    Task UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks);
    Task UpdateToDoItemYearsOffsetAsync(Guid id, ushort years);
    Task UpdateToDoItemChildrenTypeAsync(Guid id, ToDoItemChildrenType type);
    Task<IEnumerable<ToDoShortItem>> GetSiblingsAsync(Guid id);
    Task<ActiveToDoItem?> GetActiveToDoItemAsync(TimeSpan offset);
    Task UpdateToDoItemLinkAsync(Guid id, Uri? link);
}
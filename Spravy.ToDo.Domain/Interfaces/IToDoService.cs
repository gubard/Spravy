using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Domain.Interfaces;

public interface IToDoService
{
    Task<IEnumerable<IToDoSubItem>> GetRootToDoSubItemsAsync();
    Task<IToDoItem> GetToDoItemAsync(Guid id);
    Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options);
    Task<Guid> AddToDoItemAsync(AddToDoItemOptions options);
    Task DeleteToDoItemAsync(Guid id);
    Task UpdateToDoItemTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type);
    Task UpdateToDoItemDueDateAsync(Guid id, DateTimeOffset dueDate);
    Task UpdateToDoItemNameAsync(Guid id, string name);
    Task UpdateToDoItemOrderIndexAsync(UpdateOrderIndexToDoItemOptions options);
    Task UpdateToDoItemDescriptionAsync(Guid id, string description);
    Task UpdateToDoItemCompleteStatusAsync(Guid id, bool isCompleted);
    Task SkipToDoItemAsync(Guid id);
    Task FailToDoItemAsync(Guid id);
    Task<IEnumerable<IToDoSubItem>> SearchToDoSubItemsAsync(string searchText);
    Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type);
    Task AddCurrentToDoItemAsync(Guid id);
    Task RemoveCurrentToDoItemAsync(Guid id);
    Task<IEnumerable<IToDoSubItem>> GetCurrentToDoItemsAsync();
    Task UpdateToDoItemAnnuallyPeriodicityAsync(Guid id, AnnuallyPeriodicity periodicity);
    Task UpdateToDoItemMonthlyPeriodicityAsync(Guid id, MonthlyPeriodicity periodicity);
    Task UpdateToDoItemWeeklyPeriodicityAsync(Guid id, WeeklyPeriodicity periodicity);
    Task<IEnumerable<IToDoSubItem>> GetLeafToDoSubItemsAsync(Guid id);
    Task<IEnumerable<ToDoSelectorItem>> GetToDoSelectorItemsAsync();
    Task UpdateToDoItemParentAsync(Guid id, Guid parentId);
    Task ToDoItemToRootAsync(Guid id);
    Task<string> ToDoItemToStringAsync(Guid id);
    Task InitAsync();
    Task UpdateToDoItemDaysOffsetAsync(Guid id, ushort days);
    Task UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months);
    Task UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks);
    Task UpdateToDoItemYearsOffsetAsync(Guid id, ushort years);
    Task UpdateToDoItemChildrenTypeAsync(Guid id, ToDoItemChildrenType type);
}
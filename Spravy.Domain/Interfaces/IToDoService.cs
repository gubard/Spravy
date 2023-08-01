using Spravy.Domain.Enums;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IToDoService
{
    Task<IEnumerable<IToDoSubItem>> GetRootToDoItemsAsync();
    Task<IToDoItem> GetToDoItemAsync(Guid id);
    Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options);
    Task<Guid> AddToDoItemAsync(AddToDoItemOptions options);
    Task DeleteToDoItemAsync(Guid id);
    Task UpdateTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type);
    Task UpdateDueDateAsync(Guid id, DateTimeOffset dueDate);
    Task UpdateCompleteStatusAsync(Guid id, bool isCompleted);
    Task UpdateNameToDoItemAsync(Guid id, string name);
    Task UpdateOrderIndexToDoItemAsync(UpdateOrderIndexToDoItemOptions options);
    Task UpdateDescriptionToDoItemAsync(Guid id, string description);
    Task SkipToDoItemAsync(Guid id);
    Task FailToDoItemAsync(Guid id);
    Task<IEnumerable<IToDoSubItem>> SearchAsync(string searchText);
    Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type);
    Task AddCurrentToDoItemAsync(Guid id);
    Task RemoveCurrentToDoItemAsync(Guid id);
    Task<IEnumerable<IToDoSubItem>> GetCurrentToDoItemsAsync();
    Task UpdateAnnuallyPeriodicityAsync(Guid id, AnnuallyPeriodicity periodicity);
    Task UpdateMonthlyPeriodicityAsync(Guid id, MonthlyPeriodicity periodicity);
    Task UpdateWeeklyPeriodicityAsync(Guid id, WeeklyPeriodicity periodicity);
    Task<IEnumerable<IToDoSubItem>> GetLeafToDoItemsAsync(Guid id);
}
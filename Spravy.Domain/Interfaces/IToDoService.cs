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
    Task UpdateDueDateAsync(Guid id, DateTimeOffset? dueDate);
    Task UpdateCompleteStatusAsync(Guid id, bool isComplete);
    Task UpdateNameToDoItemAsync(Guid id, string name);
    Task UpdateOrderIndexToDoItemAsync(UpdateOrderIndexToDoItemOptions options);
    Task UpdateDescriptionToDoItemAsync(Guid id, string description);
    Task SkipToDoItemAsync(Guid id);
    Task FailToDoItemAsync(Guid id);
    Task<IEnumerable<IToDoSubItem>> SearchAsync(string searchText);
    Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type);
}
using Spravy.Core.Enums;
using Spravy.Core.Models;

namespace Spravy.Core.Interfaces;

public interface IToDoService
{
    Task<IEnumerable<ToDoSubItem>> GetRootToDoItemsAsync();
    Task<ToDoItem> GetToDoItemAsync(Guid id);
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
}
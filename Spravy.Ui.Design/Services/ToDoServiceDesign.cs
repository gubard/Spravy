using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;

namespace Spravy.Ui.Design.Services;

public class ToDoServiceDesign : IToDoService
{
    private readonly IEnumerable<IToDoSubItem> roots;
    private readonly IEnumerable<IToDoSubItem> favorite;
    private readonly IToDoItem item;
    private readonly IEnumerable<ToDoShortItem> siblings;
    private readonly IEnumerable<IToDoSubItem> leafs;

    public ToDoServiceDesign(
        IEnumerable<IToDoSubItem> roots,
        IEnumerable<IToDoSubItem> favorite,
        IToDoItem item,
        IEnumerable<ToDoShortItem> siblings,
        IEnumerable<IToDoSubItem> leafs
    )
    {
        this.roots = roots;
        this.favorite = favorite;
        this.item = item;
        this.siblings = siblings;
        this.leafs = leafs;
    }

    public Task<IEnumerable<IToDoSubItem>> GetRootToDoSubItemsAsync()
    {
        return roots.ToTaskResult();
    }

    public Task<IToDoItem> GetToDoItemAsync(Guid id)
    {
        return item.ToTaskResult();
    }

    public Task<IEnumerable<IToDoSubItem>> GetRootToDoSubItemsAsync(TimeSpan offset)
    {
        throw new NotImplementedException();
    }

    public Task<IToDoItem> GetToDoItemAsync(Guid id, TimeSpan offset)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> AddToDoItemAsync(AddToDoItemOptions options, TimeSpan offset)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> AddToDoItemAsync(AddToDoItemOptions options)
    {
        throw new NotImplementedException();
    }

    public Task DeleteToDoItemAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemDueDateAsync(Guid id, DateOnly dueDate)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemDueDateAsync(Guid id, DateTimeOffset dueDate)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemNameAsync(Guid id, string name)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemOrderIndexAsync(UpdateOrderIndexToDoItemOptions options)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemDescriptionAsync(Guid id, string description)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemCompleteStatusAsync(Guid id, bool isCompleted, TimeSpan offset)
    {
        throw new NotImplementedException();
    }

    public Task SkipToDoItemAsync(Guid id, TimeSpan offset)
    {
        throw new NotImplementedException();
    }

    public Task FailToDoItemAsync(Guid id, TimeSpan offset)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IToDoSubItem>> SearchToDoSubItemsAsync(string searchText, TimeSpan offset)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemCompleteStatusAsync(Guid id, bool isCompleted)
    {
        throw new NotImplementedException();
    }

    public Task SkipToDoItemAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task FailToDoItemAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IToDoSubItem>> SearchToDoSubItemsAsync(string searchText)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type)
    {
        throw new NotImplementedException();
    }

    public Task AddFavoriteToDoItemAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFavoriteToDoItemAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IToDoSubItem>> GetFavoriteToDoItemsAsync(TimeSpan offset)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IToDoSubItem>> GetFavoriteToDoItemsAsync()
    {
        return favorite.ToTaskResult();
    }

    public Task UpdateToDoItemAnnuallyPeriodicityAsync(Guid id, AnnuallyPeriodicity periodicity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemMonthlyPeriodicityAsync(Guid id, MonthlyPeriodicity periodicity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemWeeklyPeriodicityAsync(Guid id, WeeklyPeriodicity periodicity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IToDoSubItem>> GetLeafToDoSubItemsAsync(Guid id, TimeSpan offset)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IToDoSubItem>> GetLeafToDoSubItemsAsync(Guid id)
    {
        return leafs.ToTaskResult();
    }

    public Task<IEnumerable<ToDoSelectorItem>> GetToDoSelectorItemsAsync(Guid[] ignoreIds)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemParentAsync(Guid id, Guid parentId)
    {
        throw new NotImplementedException();
    }

    public Task ToDoItemToRootAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<string> ToDoItemToStringAsync(ToDoItemToStringOptions options, TimeSpan offset)
    {
        throw new NotImplementedException();
    }

    public Task<string> ToDoItemToStringAsync(ToDoItemToStringOptions options)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemDaysOffsetAsync(Guid id, ushort days)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemYearsOffsetAsync(Guid id, ushort years)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemChildrenTypeAsync(Guid id, ToDoItemChildrenType type)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ToDoShortItem>> GetSiblingsAsync(Guid id)
    {
        return siblings.ToTaskResult();
    }

    public Task<ActiveToDoItem?> GetActiveToDoItemAsync(TimeSpan offset)
    {
        throw new NotImplementedException();
    }

    public Task<ActiveToDoItem?> GetActiveToDoItemAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateToDoItemLinkAsync(Guid id, Uri? link)
    {
        throw new NotImplementedException();
    }
}
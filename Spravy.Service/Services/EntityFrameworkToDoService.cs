using AutoMapper;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.Common.Models;
using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Db.Contexts;
using Spravy.Db.Core.Profiles;
using Spravy.Db.Extension;
using Spravy.Db.Models;

namespace Spravy.Service.Services;

public class EntityFrameworkToDoService : IToDoService
{
    private readonly SpravyDbContext context;
    private readonly IMapper mapper;

    public EntityFrameworkToDoService(SpravyDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<IToDoSubItem>> GetRootToDoSubItemsAsync()
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var result = await ConvertAsync(items);

        return result;
    }

    private async Task<IEnumerable<IToDoSubItem>> ConvertAsync(IEnumerable<ToDoItemEntity> items)
    {
        var result = new List<IToDoSubItem>();

        foreach (var item in items)
        {
            var toDoSubItem = await ConvertAsync(item);
            result.Add(toDoSubItem);
        }

        return result;
    }

    private async Task<IToDoSubItem> ConvertAsync(ToDoItemEntity item)
    {
        var (status, active) = await GetStatusAndActiveAsync(item, item.Id);

        return mapper.Map<IToDoSubItem>(
            item,
            a =>
            {
                a.Items.Add(SpravyDbProfile.StatusName, status);
                a.Items.Add(SpravyDbProfile.ActiveName, active);
            }
        );
    }

    private Task<(ToDoItemStatus Status, ActiveToDoItem? Active)> GetStatusAndActiveAsync(ToDoItemEntity entity)
    {
        switch (entity.Type)
        {
            case ToDoItemType.Value: return GetStatusAndActiveValueAsync(entity);
            case ToDoItemType.Group: return GetStatusAndActiveGroupAsync(entity);
            case ToDoItemType.Planned: return GetStatusAndActivePlannedAsync(entity);
            case ToDoItemType.Periodicity: return GetStatusAndActivePeriodicityAsync(entity);
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<(ToDoItemStatus Status, ActiveToDoItem? Active)> GetStatusAndActivePeriodicityAsync(
        ToDoItemEntity entity
    )
    {
        if (entity.DueDate < DateTimeOffset.Now.ToCurrentDay())
        {
            return (ToDoItemStatus.Miss, null);
        }

        if (entity.DueDate > DateTimeOffset.Now.ToCurrentDay())
        {
            return (ToDoItemStatus.Completed, null);
        }

        (ToDoItemStatus Status, ActiveToDoItem? Active) result = (ToDoItemStatus.ReadyForComplete, null);

        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .ToArrayAsync();

        if (children.Length == 0)
        {
            return (ToDoItemStatus.ReadyForComplete, mapper.Map<ActiveToDoItem>(entity));
        }

        var completeChildrenCount = 0;
        var currentOrder = uint.MaxValue;

        foreach (var child in children)
        {
            var status = await GetStatusAndActiveAsync(child);

            switch (status.Status)
            {
                case ToDoItemStatus.Miss:
                    return (ToDoItemStatus.Miss, mapper.Map<ActiveToDoItem>(child));
                case ToDoItemStatus.Completed:
                    completeChildrenCount++;
                    break;
                case ToDoItemStatus.ReadyForComplete:
                    if (Math.Min(currentOrder, child.OrderIndex) == child.OrderIndex)
                    {
                        currentOrder = child.OrderIndex;
                        result = (result.Status, status.Active);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (completeChildrenCount == children.Length)
        {
            return (ToDoItemStatus.ReadyForComplete, null);
        }

        if (entity.DueDate.ToCurrentDay() == DateTimeOffset.Now.ToCurrentDay())
        {
            return (ToDoItemStatus.ReadyForComplete, result.Active);
        }

        return result;
    }

    private async Task<(ToDoItemStatus Status, ActiveToDoItem? Active)> GetStatusAndActivePlannedAsync(
        ToDoItemEntity entity
    )
    {
        if (entity.IsCompleted)
        {
            return (ToDoItemStatus.Completed, null);
        }

        if (entity.DueDate < DateTimeOffset.Now.ToCurrentDay())
        {
            return (ToDoItemStatus.Miss, null);
        }

        if (entity.DueDate > DateTimeOffset.Now.ToCurrentDay())
        {
            return (ToDoItemStatus.Completed, null);
        }

        (ToDoItemStatus Status, ActiveToDoItem? Active) result = (ToDoItemStatus.ReadyForComplete, null);

        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .ToArrayAsync();

        if (children.Length == 0)
        {
            return (ToDoItemStatus.ReadyForComplete, mapper.Map<ActiveToDoItem>(entity));
        }

        var completeChildrenCount = 0;
        var currentOrder = uint.MaxValue;

        foreach (var child in children)
        {
            var status = await GetStatusAndActiveAsync(child);

            switch (status.Status)
            {
                case ToDoItemStatus.Miss:
                    return (ToDoItemStatus.Miss, mapper.Map<ActiveToDoItem>(child));
                case ToDoItemStatus.Completed:
                    completeChildrenCount++;
                    break;
                case ToDoItemStatus.ReadyForComplete:
                    if (Math.Min(currentOrder, child.OrderIndex) == child.OrderIndex)
                    {
                        currentOrder = child.OrderIndex;
                        result = (result.Status, status.Active);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (completeChildrenCount == children.Length)
        {
            return (ToDoItemStatus.ReadyForComplete, null);
        }

        if (entity.DueDate.ToCurrentDay() == DateTimeOffset.Now.ToCurrentDay())
        {
            return (ToDoItemStatus.ReadyForComplete, null);
        }

        return result;
    }

    private async Task<(ToDoItemStatus Status, ActiveToDoItem? Active)> GetStatusAndActiveAsync(
        ToDoItemEntity entity,
        Guid id
    )
    {
        var result = await GetStatusAndActiveAsync(entity);

        if (result.Active is null)
        {
            return result;
        }

        if (result.Active.Value.Id == id)
        {
            return (result.Status, null);
        }

        var item = await context.Set<ToDoItemEntity>().FindAsync(result.Active.Value.Id);
        item = item.ThrowIfNull();

        if (item.ParentId is null)
        {
            return (result.Status, null);
        }

        return (result.Status, new ActiveToDoItem(item.ParentId.Value, result.Active.Value.Name));
    }

    private async Task<(ToDoItemStatus Status, ActiveToDoItem? Active)> GetStatusAndActiveGroupAsync(
        ToDoItemEntity entity
    )
    {
        var currentOrder = uint.MaxValue;
        (ToDoItemStatus Status, ActiveToDoItem? Active) result = (ToDoItemStatus.ReadyForComplete, null);

        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .ToArrayAsync();

        if (children.Length == 0)
        {
            return (ToDoItemStatus.Completed, null);
        }

        var completeChildrenCount = 0;

        foreach (var child in children)
        {
            var status = await GetStatusAndActiveAsync(child);

            switch (status.Status)
            {
                case ToDoItemStatus.Miss:
                    return (ToDoItemStatus.Miss, mapper.Map<ActiveToDoItem>(child));
                case ToDoItemStatus.Completed:
                    completeChildrenCount++;
                    break;
                case ToDoItemStatus.ReadyForComplete:
                    if (Math.Min(currentOrder, child.OrderIndex) == child.OrderIndex)
                    {
                        currentOrder = child.OrderIndex;
                        result = (result.Status, status.Active);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (completeChildrenCount == children.Length)
        {
            return (ToDoItemStatus.Completed, null);
        }

        return result;
    }


    private async Task<(ToDoItemStatus Status, ActiveToDoItem? Active)> GetStatusAndActiveValueAsync(
        ToDoItemEntity entity
    )
    {
        if (entity.IsCompleted)
        {
            return (ToDoItemStatus.Completed, null);
        }

        (ToDoItemStatus Status, ActiveToDoItem? Active) result = (ToDoItemStatus.ReadyForComplete, null);

        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .ToArrayAsync();

        if (children.Length == 0)
        {
            return (ToDoItemStatus.ReadyForComplete, mapper.Map<ActiveToDoItem>(entity));
        }

        var completeChildrenCount = 0;
        var currentOrder = uint.MaxValue;

        foreach (var child in children)
        {
            var status = await GetStatusAndActiveAsync(child);

            switch (status.Status)
            {
                case ToDoItemStatus.Miss:
                    return (ToDoItemStatus.Miss, mapper.Map<ActiveToDoItem>(child));
                case ToDoItemStatus.Completed:
                    completeChildrenCount++;
                    break;
                case ToDoItemStatus.ReadyForComplete:
                    if (Math.Min(currentOrder, child.OrderIndex) == child.OrderIndex)
                    {
                        currentOrder = child.OrderIndex;
                        result = (result.Status, status.Active);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (completeChildrenCount == children.Length)
        {
            return (ToDoItemStatus.ReadyForComplete, null);
        }

        return result;
    }

    public async Task<IToDoItem> GetToDoItemAsync(Guid id)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        var subItems = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == item.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var parents = new List<ToDoItemParent>
        {
            new(item.Id, item.Name)
        };

        await GetParentsAsync(id, parents);
        parents.Reverse();
        var toDoSubItems = await ConvertAsync(subItems);

        var toDoItem = mapper.Map<IToDoItem>(
            item,
            opt =>
            {
                opt.Items.Add(SpravyDbProfile.ParentsName, parents.ToArray());
                opt.Items.Add(SpravyDbProfile.ItemsName, toDoSubItems.ToArray());
            }
        );

        return toDoItem;
    }

    public async Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options)
    {
        var id = Guid.NewGuid();

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == null)
            .Select(x => x.OrderIndex)
            .ToArrayAsync();

        var newEntity = mapper.Map<ToDoItemEntity>(options);
        newEntity.Description = string.Empty;
        newEntity.Id = id;
        newEntity.OrderIndex = items.Length == 0 ? 0 : items.Max() + 1;
        await context.Set<ToDoItemEntity>().AddAsync(newEntity);
        await context.SaveChangesAsync();

        return id;
    }

    public async Task<Guid> AddToDoItemAsync(AddToDoItemOptions options)
    {
        var id = Guid.NewGuid();
        var parent = await context.Set<ToDoItemEntity>().FindAsync(options.ParentId);
        parent = parent.ThrowIfNull();

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == options.ParentId)
            .Select(x => x.OrderIndex)
            .ToArrayAsync();

        var toDoItem = mapper.Map<ToDoItemEntity>(options);
        toDoItem.Description = string.Empty;
        toDoItem.Id = id;
        toDoItem.OrderIndex = items.Length == 0 ? 0 : items.Max() + 1;

        toDoItem.DueDate = parent.DueDate < DateTimeOffset.Now.ToCurrentDay()
            ? DateTimeOffset.Now.ToCurrentDay()
            : parent.DueDate;

        toDoItem.TypeOfPeriodicity = parent.TypeOfPeriodicity;
        toDoItem.DaysOfMonth = parent.DaysOfMonth;
        toDoItem.DaysOfWeek = parent.DaysOfWeek;
        toDoItem.DaysOfYear = parent.DaysOfYear;
        await context.Set<ToDoItemEntity>().AddAsync(toDoItem);
        await context.SaveChangesAsync();

        return id;
    }

    public async Task DeleteToDoItemAsync(Guid id)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        var children = await context.Set<ToDoItemEntity>().AsNoTracking().Where(x => x.ParentId == id).ToArrayAsync();

        foreach (var child in children)
        {
            await DeleteToDoItemAsync(child.Id);
        }

        context.Set<ToDoItemEntity>().Remove(item);
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.TypeOfPeriodicity = type;
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemDueDateAsync(Guid id, DateTimeOffset dueDate)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.DueDate = dueDate;
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemCompleteStatusAsync(Guid id, bool isCompleted)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        if (isCompleted)
        {
            var isCanComplete = await IsCanFinishToDoItem(item);

            if (!isCanComplete)
            {
                throw new Exception("Need close sub tasks.");
            }

            switch (item.Type)
            {
                case ToDoItemType.Value:
                    item.IsCompleted = true;
                    break;
                case ToDoItemType.Group:
                    break;
                case ToDoItemType.Planned:
                    item.IsCompleted = true;
                    break;
                case ToDoItemType.Periodicity:
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            item.CompletedCount++;
            UpdateDueDate(item);
            item.LastCompleted = DateTimeOffset.Now;
        }
        else
        {
            item.IsCompleted = false;
        }

        await context.SaveChangesAsync();
    }

    private async Task<bool> IsCanFinishToDoItem(ToDoItemEntity item)
    {
        if (item.Type == ToDoItemType.Group)
        {
            return false;
        }

        switch (item.Type)
        {
            case ToDoItemType.Planned:
                if (item.DueDate.ToCurrentDay() > DateTimeOffset.Now.ToCurrentDay())
                {
                    return false;
                }

                break;
            case ToDoItemType.Periodicity:
                if (item.DueDate.ToCurrentDay() > DateTimeOffset.Now.ToCurrentDay())
                {
                    return false;
                }

                break;
        }

        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == item.Id)
            .ToArrayAsync();

        foreach (var child in children)
        {
            var dueDate = item.Type switch
            {
                ToDoItemType.Value => (DateTimeOffset?)null,
                ToDoItemType.Group => null,
                ToDoItemType.Planned => item.DueDate,
                ToDoItemType.Periodicity => item.DueDate,
                _ => throw new ArgumentOutOfRangeException()
            };

            var isCanComplete = await IsCanFinishToDoItem(child, dueDate);

            if (!isCanComplete)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> IsCanFinishToDoSubItemPeriodicity(ToDoItemEntity item, DateTimeOffset? rootDue)
    {
        if (rootDue.HasValue)
        {
            if (item.DueDate.ToCurrentDay() <= rootDue.Value.ToCurrentDay())
            {
                return false;
            }
        }
        else
        {
            if (item.DueDate.ToCurrentDay() <= DateTimeOffset.Now.ToCurrentDay())
            {
                return false;
            }
        }

        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == item.Id)
            .ToArrayAsync();

        foreach (var child in children)
        {
            var isCanComplete = await IsCanFinishToDoItem(child, rootDue);

            if (!isCanComplete)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> IsCanFinishToDoSubItemPlanned(ToDoItemEntity item, DateTimeOffset? rootDue)
    {
        if (item.IsCompleted)
        {
            return true;
        }

        if (rootDue.HasValue)
        {
            if (item.DueDate.ToCurrentDay() <= rootDue.Value.ToCurrentDay())
            {
                return false;
            }
        }
        else
        {
            if (item.DueDate.ToCurrentDay() <= DateTimeOffset.Now.ToCurrentDay())
            {
                return false;
            }
        }

        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == item.Id)
            .ToArrayAsync();

        foreach (var child in children)
        {
            var isCanComplete = await IsCanFinishToDoItem(child, rootDue);

            if (!isCanComplete)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> IsCanFinishToDoSubItemValue(ToDoItemEntity item, DateTimeOffset? rootDue)
    {
        if (item.IsCompleted)
        {
            return true;
        }

        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == item.Id)
            .ToArrayAsync();

        foreach (var child in children)
        {
            var isCanComplete = await IsCanFinishToDoItem(child, rootDue);

            if (!isCanComplete)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> IsCanFinishToDoSubItemGroup(ToDoItemEntity item, DateTimeOffset? rootDue)
    {
        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == item.Id)
            .ToArrayAsync();

        foreach (var child in children)
        {
            var isCanComplete = await IsCanFinishToDoItem(child, rootDue);

            if (!isCanComplete)
            {
                return false;
            }
        }

        return true;
    }

    private Task<bool> IsCanFinishToDoItem(ToDoItemEntity item, DateTimeOffset? rootDue)
    {
        switch (item.Type)
        {
            case ToDoItemType.Value: return IsCanFinishToDoSubItemValue(item, rootDue);
            case ToDoItemType.Group: return IsCanFinishToDoSubItemGroup(item, rootDue);
            case ToDoItemType.Planned: return IsCanFinishToDoSubItemPlanned(item, rootDue);
            case ToDoItemType.Periodicity: return IsCanFinishToDoSubItemPeriodicity(item, rootDue);
            default: throw new ArgumentOutOfRangeException();
        }
    }

    public async Task UpdateToDoItemNameAsync(Guid id, string name)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.Name = name;
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemOrderIndexAsync(UpdateOrderIndexToDoItemOptions options)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(options.Id);
        item = item.ThrowIfNull();
        var targetItem = await context.Set<ToDoItemEntity>().FindAsync(options.TargetId);
        targetItem = targetItem.ThrowIfNull();
        var orderIndex = options.IsAfter ? targetItem.OrderIndex + 1 : targetItem.OrderIndex;

        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.ParentId && x.Id != item.Id && x.OrderIndex >= orderIndex)
            .ToArrayAsync();

        foreach (var itemEntity in items)
        {
            itemEntity.OrderIndex++;
        }

        item.OrderIndex = orderIndex;
        await context.SaveChangesAsync();
        await NormalizeOrderIndexAsync(item.ParentId);
    }

    public async Task UpdateToDoItemDescriptionAsync(Guid id, string description)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.Description = description;
        await context.SaveChangesAsync();
    }

    public async Task SkipToDoItemAsync(Guid id)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        var isCanComplete = await IsCanFinishToDoItem(item);

        if (!isCanComplete)
        {
            throw new Exception("Need close sub tasks.");
        }

        switch (item.Type)
        {
            case ToDoItemType.Value:
                item.IsCompleted = true;

                break;
            case ToDoItemType.Group:
                break;
            case ToDoItemType.Planned:
                item.IsCompleted = true;

                break;
            case ToDoItemType.Periodicity:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        item.SkippedCount++;
        UpdateDueDate(item);
        item.LastCompleted = DateTimeOffset.Now;
        await context.SaveChangesAsync();
    }

    public async Task FailToDoItemAsync(Guid id)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        var isCanComplete = await IsCanFinishToDoItem(item);

        if (!isCanComplete)
        {
            throw new Exception("Need close sub tasks.");
        }

        switch (item.Type)
        {
            case ToDoItemType.Value:
                item.IsCompleted = true;

                break;
            case ToDoItemType.Group:
                break;
            case ToDoItemType.Planned:
                item.IsCompleted = true;

                break;
            case ToDoItemType.Periodicity:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        item.FailedCount++;
        UpdateDueDate(item);
        item.LastCompleted = DateTimeOffset.Now;
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<IToDoSubItem>> SearchToDoSubItemsAsync(string searchText)
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.Name.Contains(searchText))
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var result = await ConvertAsync(items);

        return result;
    }

    public async Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.Type = type;
        await context.SaveChangesAsync();
    }

    public async Task AddCurrentToDoItemAsync(Guid id)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.IsCurrent = true;
        await context.SaveChangesAsync();
    }

    public async Task RemoveCurrentToDoItemAsync(Guid id)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.IsCurrent = false;
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<IToDoSubItem>> GetCurrentToDoItemsAsync()
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.IsCurrent)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var result = await ConvertAsync(items);

        return result;
    }

    public async Task UpdateToDoItemAnnuallyPeriodicityAsync(Guid id, AnnuallyPeriodicity periodicity)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.SetDaysOfYear(periodicity.Days);
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemMonthlyPeriodicityAsync(Guid id, MonthlyPeriodicity periodicity)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.SetDaysOfMonth(periodicity.Days);
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemWeeklyPeriodicityAsync(Guid id, WeeklyPeriodicity periodicity)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.SetDaysOfWeek(periodicity.Days);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<IToDoSubItem>> GetLeafToDoSubItemsAsync(Guid id)
    {
        var entities = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        if (entities.IsEmpty())
        {
            return Enumerable.Empty<IToDoSubItem>();
        }

        var result = new List<IToDoSubItem>();

        foreach (var entity in entities)
        {
            await foreach (var item in GetLeafToDoItemsAsync(entity))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public async Task<IEnumerable<ToDoSelectorItem>> GetToDoSelectorItemsAsync()
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var result = new ToDoSelectorItem[items.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var item = items[i];
            var children = await GetToDoSelectorItemsAsync(item.Id);
            result[i] = new ToDoSelectorItem(item.Id, item.Name, children);
        }

        return result;
    }

    public async Task UpdateToDoItemParentAsync(Guid id, Guid parentId)
    {
        if (id == parentId)
        {
            throw new Exception();
        }

        var entity = await context.Set<ToDoItemEntity>().FindAsync(id);

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == parentId)
            .Select(x => x.OrderIndex)
            .ToArrayAsync();

        entity = entity.ThrowIfNull();
        entity.ParentId = parentId;
        entity.OrderIndex = items.Length == 0 ? 0 : items.Max() + 1;
        await context.SaveChangesAsync();
    }

    public async Task ToDoItemToRootAsync(Guid id)
    {
        var entity = await context.Set<ToDoItemEntity>().FindAsync(id);

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == null)
            .Select(x => x.OrderIndex)
            .ToArrayAsync();

        entity = entity.ThrowIfNull();
        entity.ParentId = null;
        entity.OrderIndex = items.Length == 0 ? 0 : items.Max() + 1;
        await context.SaveChangesAsync();
    }

    private async Task<ToDoSelectorItem[]> GetToDoSelectorItemsAsync(Guid id)
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var result = new ToDoSelectorItem[items.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var item = items[i];
            var children = await GetToDoSelectorItemsAsync(item.Id);
            result[i] = new ToDoSelectorItem(item.Id, item.Name, children);
        }

        return result;
    }

    private async IAsyncEnumerable<IToDoSubItem> GetLeafToDoItemsAsync(ToDoItemEntity itemEntity)
    {
        var entities = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == itemEntity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        if (entities.IsEmpty())
        {
            yield return await ConvertAsync(itemEntity);
            yield break;
        }

        foreach (var entity in entities)
        {
            await foreach (var item in GetLeafToDoItemsAsync(entity))
            {
                yield return item;
            }
        }
    }

    private async Task NormalizeOrderIndexAsync(Guid? parentId)
    {
        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == parentId)
            .ToArrayAsync();

        var ordered = items.OrderBy(x => x.OrderIndex).ToArray();

        for (var index = 0u; index < ordered.LongLength; index++)
        {
            ordered[index].OrderIndex = index;
        }

        await context.SaveChangesAsync();
    }

    private async Task GetParentsAsync(Guid id, List<ToDoItemParent> parents)
    {
        var parent = await context.Set<ToDoItemEntity>().Include(x => x.Parent).SingleAsync(x => x.Id == id);

        if (parent.Parent is null)
        {
            return;
        }

        parents.Add(mapper.Map<ToDoItemParent>(parent.Parent));
        await GetParentsAsync(parent.Parent.Id, parents);
    }

    private void UpdateDueDate(ToDoItemEntity item)
    {
        switch (item.Type)
        {
            case ToDoItemType.Value:
                break;
            case ToDoItemType.Group:
                break;
            case ToDoItemType.Planned:
                break;
            case ToDoItemType.Periodicity:
                AddPeriodicity(item);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddPeriodicity(ToDoItemEntity item)
    {
        switch (item.TypeOfPeriodicity)
        {
            case TypeOfPeriodicity.Daily:
                item.DueDate = item.DueDate.AddDays(1);

                break;
            case TypeOfPeriodicity.Weekly:
            {
                var dayOfWeek = item.DueDate.DayOfWeek;
                var daysOfWeek = item.GetDaysOfWeek().Order().Select(x => (DayOfWeek?)x).ThrowIfEmpty().ToArray();
                var nextDay = daysOfWeek.FirstOrDefault(x => x > dayOfWeek);

                item.DueDate = nextDay is not null
                    ? item.DueDate.AddDays((double)nextDay - (double)dayOfWeek)
                    : item.DueDate.AddDays(7 - (double)dayOfWeek + (double)daysOfWeek.First());

                break;
            }
            case TypeOfPeriodicity.Monthly:
            {
                var now = item.DueDate;
                var dayOfMonth = now.Day;
                var daysOfMonth = item.GetDaysOfMonth().Order().Select(x => (byte?)x).ThrowIfEmpty().ToArray();
                var nextDay = daysOfMonth.FirstOrDefault(x => x > dayOfMonth);
                var daysInCurrentMonth = DateTime.DaysInMonth(now.Year, now.Month);
                var daysInNextMonth = DateTime.DaysInMonth(now.AddMonths(1).Year, now.AddMonths(1).Month);

                item.DueDate = nextDay is not null
                    ? item.DueDate.WithDay(Math.Min(nextDay.Value, daysInCurrentMonth))
                    : item.DueDate.AddMonths(1).WithDay(Math.Min(daysOfMonth.First().Value, daysInNextMonth));

                break;
            }
            case TypeOfPeriodicity.Annually:
            {
                var now = item.DueDate;
                var daysOfYear = item.GetDaysOfYear().Order().Select(x => (DayOfYear?)x).ThrowIfEmpty().ToArray();
                var nextDay = daysOfYear.FirstOrDefault(x => x.Value.Month >= now.Month && x.Value.Day > now.Day);
                var daysInNextMonth = DateTime.DaysInMonth(now.Year + 1, daysOfYear.First().Value.Month);

                item.DueDate = nextDay is not null
                    ? item.DueDate.WithMonth(nextDay.Value.Month)
                        .WithDay(Math.Min(DateTime.DaysInMonth(now.Year, nextDay.Value.Month), nextDay.Value.Day))
                    : item.DueDate.AddYears(1)
                        .WithMonth(daysOfYear.First().Value.Month)
                        .WithDay(Math.Min(daysInNextMonth, daysOfYear.First().Value.Day));

                break;
            }
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
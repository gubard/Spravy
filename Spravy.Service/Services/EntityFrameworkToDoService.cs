using AutoMapper;
using ExtensionFramework.Core.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Db.Contexts;
using Spravy.Db.Core.Profiles;
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

    public async Task<IEnumerable<IToDoSubItem>> GetRootToDoItemsAsync()
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == null)
            .Include(x => x.Value)
            .Include(x => x.Group)
            .Include(x => x.Statistical)
            .ToArrayAsync();

        var result = await ConvertAsync(items);

        return result;
    }

    private async Task<IEnumerable<IToDoSubItem>> ConvertAsync(IEnumerable<ToDoItemEntity> items)
    {
        var result = new List<IToDoSubItem>();

        foreach (var item in items)
        {
            var (status, active) = await GetStatusAndActiveAsync(item, item.Id);

            var toDoSubItem = mapper.Map<IToDoSubItem>(
                item,
                a =>
                {
                    a.Items.Add(SpravyDbProfile.StatusName, status);
                    a.Items.Add(SpravyDbProfile.ActiveName, active);
                }
            );

            result.Add(toDoSubItem);
        }

        return result;
    }

    private Task<(ToDoItemStatus Status, ActiveToDoItem? Active)> GetStatusAndActiveAsync(ToDoItemEntity entity)
    {
        switch (entity.Type)
        {
            case ToDoItemType.Value: return GetStatusAsync(entity, entity.Value);
            case ToDoItemType.Group: return GetStatusAsync(entity, entity.Group);
            default: throw new ArgumentOutOfRangeException();
        }
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

    private async Task<(ToDoItemStatus Status, ActiveToDoItem? Active)> GetStatusAsync(
        ToDoItemEntity entity,
        ToDoItemGroupEntity _
    )
    {
        var currentOrder = uint.MaxValue;
        (ToDoItemStatus Status, ActiveToDoItem? Active) result = (ToDoItemStatus.Waiting, null);

        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Include(x => x.Value)
            .Include(x => x.Group)
            .Include(x => x.Statistical)
            .Where(x => x.ParentId == entity.Id)
            .ToArrayAsync();

        if (children.Length == 0)
        {
            return (ToDoItemStatus.Complete, null);
        }

        var completeChildrenCount = 0;

        foreach (var child in children)
        {
            var status = await GetStatusAndActiveAsync(child);

            switch (status.Status)
            {
                case ToDoItemStatus.Waiting:
                    if (Math.Min(currentOrder, child.OrderIndex) == child.OrderIndex)
                    {
                        currentOrder = child.OrderIndex;
                        result = (result.Status, status.Active);
                    }

                    break;
                case ToDoItemStatus.Today:
                    result = status;
                    break;
                case ToDoItemStatus.Miss:
                    return (ToDoItemStatus.Miss, mapper.Map<ActiveToDoItem>(child));
                case ToDoItemStatus.Complete:
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
            return (ToDoItemStatus.Complete, null);
        }

        return result;
    }

    private async Task<(ToDoItemStatus Status, ActiveToDoItem? Active)> GetStatusAsync(
        ToDoItemEntity entity,
        ToDoItemValueEntity value
    )
    {
        if (value.IsComplete)
        {
            return (ToDoItemStatus.Complete, null);
        }

        if (value.DueDate.HasValue && value.DueDate.Value < DateTimeOffset.Now.ToCurrentDay())
        {
            return (ToDoItemStatus.Complete, null);
        }

        if (value.DueDate.HasValue && value.DueDate.Value > DateTimeOffset.Now.ToCurrentDay())
        {
            return (ToDoItemStatus.Complete, null);
        }

        (ToDoItemStatus Status, ActiveToDoItem? Active) result = (ToDoItemStatus.Waiting, null);

        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Include(x => x.Value)
            .Include(x => x.Statistical)
            .Include(x => x.Group)
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
            if (value.IsComplete)
            {
                continue;
            }

            var status = await GetStatusAndActiveAsync(child);

            switch (status.Status)
            {
                case ToDoItemStatus.Waiting:
                    if (Math.Min(currentOrder, child.OrderIndex) == child.OrderIndex)
                    {
                        currentOrder = child.OrderIndex;
                        result = (result.Status, status.Active);
                    }

                    break;
                case ToDoItemStatus.Today:
                    result = status;
                    break;
                case ToDoItemStatus.Miss:
                    return (ToDoItemStatus.Miss, mapper.Map<ActiveToDoItem>(child));
                case ToDoItemStatus.Complete:
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

        if (value.DueDate.HasValue && value.DueDate.Value == DateTimeOffset.Now.ToCurrentDay())
        {
            return (ToDoItemStatus.Today, null);
        }

        return result;
    }

    public async Task<IToDoItem> GetToDoItemAsync(Guid id)
    {
        var (item, value, _, _) = await GetToDoItemEntityAsync(id);

        var subItems = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == item.Id)
            .Include(x => x.Group)
            .Include(x => x.Value)
            .Include(x => x.Statistical)
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
                opt.Items.Add(SpravyDbProfile.ValueName, value);
            }
        );

        return toDoItem;
    }

    public async Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options)
    {
        var id = Guid.NewGuid();
        var items = await context.Set<ToDoItemEntity>().AsNoTracking().Where(x => x.ParentId == null).ToArrayAsync();
        var newEntity = mapper.Map<ToDoItemEntity>(options);
        newEntity.Description = string.Empty;
        newEntity.Id = id;
        newEntity.OrderIndex = items.Length == 0 ? 0 : items.Max(x => x.OrderIndex) + 1;

        var toDoItemValue = new ToDoItemValueEntity
        {
            Id = Guid.NewGuid(),
            ItemId = id,
        };

        var toDoItemGroup = new ToDoItemGroupEntity
        {
            Id = Guid.NewGuid(),
            ItemId = id,
        };

        newEntity.GroupId = toDoItemGroup.Id;
        newEntity.ValueId = toDoItemValue.Id;
        await context.Set<ToDoItemValueEntity>().AddAsync(toDoItemValue);
        await context.Set<ToDoItemGroupEntity>().AddAsync(toDoItemGroup);
        await context.Set<ToDoItemEntity>().AddAsync(newEntity);
        await context.SaveChangesAsync();

        return id;
    }

    public async Task<Guid> AddToDoItemAsync(AddToDoItemOptions options)
    {
        var id = Guid.NewGuid();
        var (parentItem, parentValue, parentGroup, statistical) = await GetToDoItemEntityAsync(options.ParentId);

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == options.ParentId)
            .ToArrayAsync();

        var toDoItemValue = new ToDoItemValueEntity
        {
            Id = Guid.NewGuid(),
            ItemId = id,
            TypeOfPeriodicity = parentValue.TypeOfPeriodicity,
            DueDate = parentValue.DueDate,
        };

        var toDoItemGroup = new ToDoItemGroupEntity
        {
            Id = Guid.NewGuid(),
            ItemId = id,
        };

        await context.Set<ToDoItemValueEntity>().AddAsync(toDoItemValue);
        await context.Set<ToDoItemGroupEntity>().AddAsync(toDoItemGroup);
        var toDoItem = mapper.Map<ToDoItemEntity>(options);
        toDoItem.Description = string.Empty;
        toDoItem.Id = id;
        toDoItem.OrderIndex = items.Length == 0 ? 0 : items.Max(x => x.OrderIndex) + 1;
        toDoItem.ValueId = toDoItemValue.Id;
        toDoItem.GroupId = toDoItemGroup.Id;

        switch (parentItem.Type)
        {
            case ToDoItemType.Value:
                toDoItemValue.TypeOfPeriodicity = parentValue.TypeOfPeriodicity;
                toDoItemValue.DueDate = parentValue.DueDate;

                break;
            case ToDoItemType.Group: break;
            default: throw new ArgumentOutOfRangeException();
        }

        await context.Set<ToDoItemEntity>().AddAsync(toDoItem);
        await context.SaveChangesAsync();

        return id;
    }

    public async Task DeleteToDoItemAsync(Guid id)
    {
        var (item, value, group, statistical) = await GetToDoItemEntityAsync(id);
        var children = await context.Set<ToDoItemEntity>().AsNoTracking().Where(x => x.ParentId == id).ToArrayAsync();

        foreach (var child in children)
        {
            await DeleteToDoItemAsync(child.Id);
        }

        context.Set<ToDoItemValueEntity>().Remove(value);
        context.Set<ToDoItemGroupEntity>().Remove(group);
        context.Set<ToDoItemEntity>().Remove(item);
        await context.SaveChangesAsync();
    }

    public async Task UpdateTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type)
    {
        var (_, value, _, _) = await GetToDoItemEntityAsync(id);
        value.TypeOfPeriodicity = type;

        if (value.DueDate is null)
        {
            switch (type)
            {
                case TypeOfPeriodicity.None:
                    break;
                case TypeOfPeriodicity.Daily:
                    value.DueDate = DateTimeOffset.Now.ToCurrentDay();
                    break;
                case TypeOfPeriodicity.Weekly:
                    value.DueDate = DateTimeOffset.Now.ToCurrentDay();
                    break;
                case TypeOfPeriodicity.Monthly:
                    value.DueDate = DateTimeOffset.Now.ToCurrentDay();
                    break;
                case TypeOfPeriodicity.Annually:
                    value.DueDate = DateTimeOffset.Now.ToCurrentDay();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        await context.SaveChangesAsync();
    }

    public async Task UpdateDueDateAsync(Guid id, DateTimeOffset? dueDate)
    {
        var (_, value, _, _) = await GetToDoItemEntityAsync(id);
        value.DueDate = dueDate;

        if (dueDate is null)
        {
            value.TypeOfPeriodicity = TypeOfPeriodicity.None;
        }

        await context.SaveChangesAsync();
    }

    public async Task UpdateCompleteStatusAsync(Guid id, bool isComplete)
    {
        var (item, value, _, statistical) = await GetToDoItemEntityAsync(id);

        if (isComplete)
        {
            var isCanComplete = await IsCanCompleteOrSkipToDoItem(item, value);

            if (!isCanComplete)
            {
                throw new Exception("Need close sub tasks.");
            }

            statistical.CompletedCount++;
            UpdateCompleteStatus(value);
        }
        else
        {
            value.IsComplete = false;
        }

        await context.SaveChangesAsync();
    }

    private Task<bool> IsCanCompleteOrSkipToDoItem(ToDoItemEntity item, ToDoItemValueEntity value)
    {
        if (item.Type == ToDoItemType.Group)
        {
            return Task.FromResult(false);
        }

        if (value.DueDate.HasValue)
        {
            if (value.DueDate.Value.ToCurrentDay() > DateTimeOffset.Now.ToCurrentDay())
            {
                return Task.FromResult(false);
            }
        }

        return IsCanCompleteOrSkipToDoItem(item, value.DueDate);
    }

    private async Task<bool> IsCanCompleteOrSkipToDoItem(ToDoItemEntity item, DateTimeOffset? rootDue)
    {
        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Include(x => x.Group)
            .Include(x => x.Value)
            .Include(x => x.Statistical)
            .Where(x => x.ParentId == item.Id)
            .ToArrayAsync();

        foreach (var child in children)
        {
            if (child.Type == ToDoItemType.Value)
            {
                if (child.Value.IsComplete)
                {
                    continue;
                }

                if (!child.Value.DueDate.HasValue)
                {
                    return false;
                }

                if (rootDue.HasValue)
                {
                    if (child.Value.DueDate.Value.ToCurrentDay() <= rootDue.Value.ToCurrentDay())
                    {
                        return false;
                    }
                }
                else
                {
                    if (child.Value.DueDate.Value.ToCurrentDay() <= DateTimeOffset.Now.ToCurrentDay())
                    {
                        return false;
                    }
                }
            }

            var isCanComplete = await IsCanCompleteOrSkipToDoItem(child, rootDue);

            if (!isCanComplete)
            {
                return false;
            }
        }

        return true;
    }

    public async Task UpdateNameToDoItemAsync(Guid id, string name)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.Name = name;
        await context.SaveChangesAsync();
    }

    public async Task UpdateOrderIndexToDoItemAsync(UpdateOrderIndexToDoItemOptions options)
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

    public async Task UpdateDescriptionToDoItemAsync(Guid id, string description)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.Description = description;
        await context.SaveChangesAsync();
    }

    public async Task SkipToDoItemAsync(Guid id)
    {
        var (item, value, group, statistical) = await GetToDoItemEntityAsync(id);
        var isCanComplete = await IsCanCompleteOrSkipToDoItem(item, value);

        if (!isCanComplete)
        {
            throw new Exception("Need close sub tasks.");
        }

        statistical.SkippedCount++;
        UpdateCompleteStatus(value);
        await context.SaveChangesAsync();
    }

    public async Task FailToDoItemAsync(Guid id)
    {
        var (item, value, group, statistical) = await GetToDoItemEntityAsync(id);
        var isCanComplete = await IsCanCompleteOrSkipToDoItem(item, value);

        if (!isCanComplete)
        {
            throw new Exception("Need close sub tasks.");
        }

        statistical.FailedCount++;
        UpdateCompleteStatus(value);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<IToDoSubItem>> SearchAsync(string searchText)
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.Name.ToUpperInvariant().Contains(searchText.ToUpperInvariant()))
            .Include(x => x.Group)
            .Include(x => x.Value)
            .Include(x => x.Statistical)
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
            .Include(x => x.Group)
            .Include(x => x.Value)
            .Include(x => x.Statistical)
            .ToArrayAsync();

        var result = await ConvertAsync(items);

        return result;
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

    private void UpdateCompleteStatus(ToDoItemValueEntity value)
    {
        if (value.TypeOfPeriodicity == TypeOfPeriodicity.None || value.DueDate is null)
        {
            value.IsComplete = true;

            return;
        }

        switch (value.TypeOfPeriodicity)
        {
            case TypeOfPeriodicity.Daily:
                value.DueDate = value.DueDate.Value.AddDays(1);
                break;
            case TypeOfPeriodicity.Weekly:
                value.DueDate = value.DueDate.Value.AddDays(7);
                break;
            case TypeOfPeriodicity.Monthly:
                value.DueDate = value.DueDate.Value.AddMonths(1);
                break;
            case TypeOfPeriodicity.Annually:
                value.DueDate = value.DueDate.Value.AddYears(1);
                break;
            default: throw new ArgumentOutOfRangeException();
        }

        value.IsComplete = false;
    }

    private async Task<(ToDoItemEntity Item,
            ToDoItemValueEntity Value,
            ToDoItemGroupEntity Group,
            ToDoItemStatisticalEntity Statistical)>
        GetToDoItemEntityAsync(Guid id)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        var value = await context.Set<ToDoItemValueEntity>().FindAsync(item.ValueId);
        var group = await context.Set<ToDoItemGroupEntity>().FindAsync(item.GroupId);
        var statistical = await context.Set<ToDoItemStatisticalEntity>().FindAsync(item.StatisticalId);

        return (item, value.ThrowIfNull(), group.ThrowIfNull(), statistical.ThrowIfNull());
    }
}
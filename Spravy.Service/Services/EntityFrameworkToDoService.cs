using AutoMapper;
using ExtensionFramework.Core.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Spravy.Core.Enums;
using Spravy.Core.Interfaces;
using Spravy.Core.Models;
using Spravy.Db.Contexts;
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

    public async Task<IEnumerable<ToDoSubItem>> GetRootToDoItemsAsync()
    {
        var items = await context.Set<ToDoItemEntity>().Where(x => x.ParentId == null).ToArrayAsync();
        var result = new List<ToDoSubItem>();

        foreach (var item in items)
        {
            var status = await GetStatusAsync(item);

            var toDoSubItem = new ToDoSubItem(
                item.Id,
                item.Name,
                item.IsComplete,
                item.DueDate,
                item.OrderIndex,
                status,
                item.Description,
                item.CompletedCount,
                item.SkippedCount,
                item.FailedCount
            );

            result.Add(toDoSubItem);
        }

        return result;
    }

    private async Task<ToDoItemStatus> GetStatusAsync(ToDoItemEntity entity)
    {
        if (entity.IsComplete)
        {
            return ToDoItemStatus.Complete;
        }

        if (entity.DueDate.HasValue && entity.DueDate.Value < DateTimeOffset.Now.ToCurrentDay())
        {
            return ToDoItemStatus.Miss;
        }

        if (entity.DueDate.HasValue && entity.DueDate.Value > DateTimeOffset.Now.ToCurrentDay())
        {
            return ToDoItemStatus.Complete;
        }

        var result = ToDoItemStatus.Waiting;

        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .ToArrayAsync();

        if (children.Length == 0)
        {
            return ToDoItemStatus.ReadyForComplete;
        }

        if (children.All(x => x.IsComplete))
        {
            return ToDoItemStatus.ReadyForComplete;
        }

        var completeChildrenCount = 0;

        foreach (var child in children)
        {
            if (child.IsComplete)
            {
                continue;
            }

            var status = await GetStatusAsync(child);

            switch (status)
            {
                case ToDoItemStatus.Waiting:
                    break;
                case ToDoItemStatus.Today:
                    result = status;
                    break;
                case ToDoItemStatus.Miss:
                    return ToDoItemStatus.Miss;
                case ToDoItemStatus.Complete:
                    completeChildrenCount++;
                    break;
                case ToDoItemStatus.ReadyForComplete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (completeChildrenCount == children.Length)
        {
            return ToDoItemStatus.ReadyForComplete;
        }

        if (entity.DueDate.HasValue && entity.DueDate.Value == DateTimeOffset.Now.ToCurrentDay())
        {
            result = ToDoItemStatus.Today;
        }

        return result;
    }

    public async Task<ToDoItem> GetToDoItemAsync(Guid id)
    {
        var item = (await context.Set<ToDoItemEntity>().FindAsync(id)).ThrowIfNull();
        var subItems = await context.Set<ToDoItemEntity>().Where(x => x.ParentId == item.Id).ToArrayAsync();

        var parents = new List<ToDoItemParent>
        {
            new(item.Id, item.Name)
        };

        await GetParentsAsync(id, parents);
        parents.Reverse();

        var toDoSubItems = new List<ToDoSubItem>();

        foreach (var subItem in subItems)
        {
            var status = await GetStatusAsync(subItem);

            var toDoSubItem = new ToDoSubItem(
                subItem.Id,
                subItem.Name,
                subItem.IsComplete,
                subItem.DueDate,
                subItem.OrderIndex,
                status,
                subItem.Description,
                subItem.CompletedCount,
                subItem.SkippedCount,
                subItem.FailedCount
            );

            toDoSubItems.Add(toDoSubItem);
        }

        return new ToDoItem(
            item.Id,
            item.Name,
            item.TypeOfPeriodicity,
            item.DueDate,
            toDoSubItems.ToArray(),
            parents.ToArray(),
            item.IsComplete,
            item.Description
        );
    }

    public async Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options)
    {
        var id = Guid.NewGuid();
        var items = await context.Set<ToDoItemEntity>().AsNoTracking().Where(x => x.ParentId == null).ToArrayAsync();
        var newEntity = mapper.Map<ToDoItemEntity>(options);
        newEntity.Description = string.Empty;
        newEntity.Id = id;
        newEntity.OrderIndex = items.Length == 0 ? 0 : items.Max(x => x.OrderIndex) + 1;
        await context.Set<ToDoItemEntity>().AddAsync(newEntity);
        await context.SaveChangesAsync();

        return id;
    }

    public async Task<Guid> AddToDoItemAsync(AddToDoItemOptions options)
    {
        var id = Guid.NewGuid();
        var parent = await context.Set<ToDoItemEntity>().FindAsync(options.ParentId);

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == options.ParentId)
            .ToArrayAsync();

        var newEntity = mapper.Map<ToDoItemEntity>(options);
        newEntity.Description = string.Empty;
        newEntity.Id = id;
        newEntity.OrderIndex = items.Length == 0 ? 0 : items.Max(x => x.OrderIndex) + 1;
        newEntity.TypeOfPeriodicity = parent.TypeOfPeriodicity;
        newEntity.DueDate = parent.DueDate;
        await context.Set<ToDoItemEntity>().AddAsync(newEntity);
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

    public async Task UpdateTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.TypeOfPeriodicity = type;

        if (item.DueDate is null)
        {
            switch (type)
            {
                case TypeOfPeriodicity.None:
                    break;
                case TypeOfPeriodicity.Daily:
                    item.DueDate = DateTimeOffset.Now.ToCurrentDay();
                    break;
                case TypeOfPeriodicity.Weekly:
                    item.DueDate = DateTimeOffset.Now.ToCurrentDay();
                    break;
                case TypeOfPeriodicity.Monthly:
                    item.DueDate = DateTimeOffset.Now.ToCurrentDay();
                    break;
                case TypeOfPeriodicity.Annually:
                    item.DueDate = DateTimeOffset.Now.ToCurrentDay();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        await context.SaveChangesAsync();
    }

    public async Task UpdateDueDateAsync(Guid id, DateTimeOffset? dueDate)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.DueDate = dueDate;

        if (dueDate is null)
        {
            item.TypeOfPeriodicity = TypeOfPeriodicity.None;
        }

        await context.SaveChangesAsync();
    }

    public async Task UpdateCompleteStatusAsync(Guid id, bool isComplete)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        if (isComplete)
        {
            var isCanComplete = await IsCanCompleteOrSkipToDoItem(item);

            if (!isCanComplete)
            {
                throw new Exception("Need close sub tasks.");
            }

            item.CompletedCount++;
            UpdateCompleteStatus(item);
        }
        else
        {
            item.IsComplete = false;
        }

        await context.SaveChangesAsync();
    }

    private Task<bool> IsCanCompleteOrSkipToDoItem(ToDoItemEntity item)
    {
        if (item.DueDate.HasValue)
        {
            if (item.DueDate.Value.ToCurrentDay() > DateTimeOffset.Now.ToCurrentDay())
            {
                return Task.FromResult(false);
            }
        }

        return IsCanCompleteOrSkipToDoItem(item, item.DueDate);
    }

    private async Task<bool> IsCanCompleteOrSkipToDoItem(ToDoItemEntity item, DateTimeOffset? rootDue)
    {
        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == item.Id)
            .ToArrayAsync();

        foreach (var child in children)
        {
            if (child.IsComplete)
            {
                continue;
            }

            if (!child.DueDate.HasValue)
            {
                return false;
            }

            if (rootDue.HasValue)
            {
                if (child.DueDate.Value.ToCurrentDay() <= rootDue.Value.ToCurrentDay())
                {
                    return false;
                }
            }
            else
            {
                if (child.DueDate.Value.ToCurrentDay() <= DateTimeOffset.Now.ToCurrentDay())
                {
                    return false;
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
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        var isCanComplete = await IsCanCompleteOrSkipToDoItem(item);

        if (!isCanComplete)
        {
            throw new Exception("Need close sub tasks.");
        }

        item.SkippedCount++;
        UpdateCompleteStatus(item);
        await context.SaveChangesAsync();
    }

    public async Task FailToDoItemAsync(Guid id)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        var isCanComplete = await IsCanCompleteOrSkipToDoItem(item);

        if (!isCanComplete)
        {
            throw new Exception("Need close sub tasks.");
        }

        item.FailedCount++;
        UpdateCompleteStatus(item);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ToDoSubItem>> SearchAsync(string searchText)
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.Name.Contains(searchText))
            .ToArrayAsync();

        var result = new List<ToDoSubItem>();

        foreach (var item in items)
        {
            var status = await GetStatusAsync(item);

            var toDoSubItem = new ToDoSubItem(
                item.Id,
                item.Name,
                item.IsComplete,
                item.DueDate,
                item.OrderIndex,
                status,
                item.Description,
                item.CompletedCount,
                item.SkippedCount,
                item.FailedCount
            );

            result.Add(toDoSubItem);
        }

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

    public void UpdateCompleteStatus(ToDoItemEntity item)
    {
        if (item.TypeOfPeriodicity == TypeOfPeriodicity.None || item.DueDate is null)
        {
            item.IsComplete = true;

            return;
        }

        switch (item.TypeOfPeriodicity)
        {
            case TypeOfPeriodicity.Daily:
                item.DueDate = item.DueDate.Value.AddDays(1);
                break;
            case TypeOfPeriodicity.Weekly:
                item.DueDate = item.DueDate.Value.AddDays(7);
                break;
            case TypeOfPeriodicity.Monthly:
                item.DueDate = item.DueDate.Value.AddMonths(1);
                break;
            case TypeOfPeriodicity.Annually:
                item.DueDate = item.DueDate.Value.AddYears(1);
                break;
            default: throw new ArgumentOutOfRangeException();
        }

        item.IsComplete = false;
    }
}
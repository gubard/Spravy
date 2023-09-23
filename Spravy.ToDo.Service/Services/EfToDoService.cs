using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.ToDo.Db.Mapper.Profiles;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Extensions;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Db.Services;

namespace Spravy.ToDo.Service.Services;

public class EfToDoService : IToDoService
{
    private readonly IMapper mapper;
    private readonly IDbContextFactory<SpravyToDoDbContext> dbContextFactory;
    private readonly StatusToDoItemService statusToDoItemService;
    private readonly ActiveToDoItemToDoItemService activeToDoItemToDoItemService;

    public EfToDoService(
        IMapper mapper,
        IDbContextFactory<SpravyToDoDbContext> dbContextFactory,
        StatusToDoItemService statusToDoItemService,
        ActiveToDoItemToDoItemService activeToDoItemToDoItemService
    )
    {
        this.mapper = mapper;
        this.dbContextFactory = dbContextFactory;
        this.statusToDoItemService = statusToDoItemService;
        this.activeToDoItemToDoItemService = activeToDoItemToDoItemService;
    }

    public async Task<IEnumerable<IToDoSubItem>> GetRootToDoSubItemsAsync()
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var result = await ConvertAsync(context, items);

        return result;
    }

    private async Task<IEnumerable<IToDoSubItem>> ConvertAsync(
        SpravyToDoDbContext context,
        IEnumerable<ToDoItemEntity> items
    )
    {
        var result = new List<IToDoSubItem>();

        foreach (var item in items)
        {
            var toDoSubItem = await ConvertAsync(context, item);
            result.Add(toDoSubItem);
        }

        return result;
    }

    private async Task<IToDoSubItem> ConvertAsync(SpravyToDoDbContext context, ToDoItemEntity item)
    {
        var status = await statusToDoItemService.GetStatusAsync(context, item);
        var active = await activeToDoItemToDoItemService.GetActiveItemAsync(context, item);

        var result = mapper.Map<IToDoSubItem>(
            item,
            a =>
            {
                a.Items.Add(SpravyToDoDbProfile.StatusName, status);
                a.Items.Add(SpravyToDoDbProfile.ActiveName, active);
            }
        );

        return result;
    }

    public async Task<IToDoItem> GetToDoItemAsync(Guid id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
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

        await GetParentsAsync(context, id, parents);
        parents.Reverse();
        var toDoSubItems = await ConvertAsync(context, subItems);

        var toDoItem = mapper.Map<IToDoItem>(
            item,
            opt =>
            {
                opt.Items.Add(SpravyToDoDbProfile.ParentsName, parents.ToArray());
                opt.Items.Add(SpravyToDoDbProfile.ItemsName, toDoSubItems.ToArray());
            }
        );

        return toDoItem;
    }

    public async Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
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
        await using var context = await dbContextFactory.CreateDbContextAsync();
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
        toDoItem.WeeksOffset = parent.WeeksOffset;
        toDoItem.DaysOffset = parent.DaysOffset;
        toDoItem.MonthsOffset = parent.MonthsOffset;
        toDoItem.YearsOffset = parent.YearsOffset;
        await context.Set<ToDoItemEntity>().AddAsync(toDoItem);
        await context.SaveChangesAsync();

        return id;
    }

    public async Task DeleteToDoItemAsync(Guid id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        var children = await context.Set<ToDoItemEntity>().AsNoTracking().Where(x => x.ParentId == id).ToArrayAsync();

        foreach (var child in children)
        {
            await DeleteToDoItemAsync(child.Id);
        }

        context.Set<ToDoItemEntity>().Remove(item);
        await NormalizeOrderIndexAsync(context, item.ParentId);
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.TypeOfPeriodicity = type;
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemDueDateAsync(Guid id, DateTimeOffset dueDate)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.DueDate = dueDate;
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemCompleteStatusAsync(Guid id, bool isCompleted)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        if (isCompleted)
        {
            switch (item.ChildrenType)
            {
                case ToDoItemChildrenType.RequireCompletion:
                {
                    var isCanComplete = await IsCanFinishToDoItem(context, item);

                    if (!isCanComplete)
                    {
                        throw new Exception("Need close sub tasks.");
                    }

                    break;
                }
                case ToDoItemChildrenType.IgnoreCompletion:
                    break;
                case ToDoItemChildrenType.CircleCompletion:
                {
                    var isCanComplete = await IsCanFinishToDoItem(context, item);

                    if (!isCanComplete)
                    {
                        throw new Exception("Need close sub tasks.");
                    }

                    break;
                }
                default: throw new ArgumentOutOfRangeException();
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
                case ToDoItemType.PeriodicityOffset:
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            item.CompletedCount++;
            UpdateDueDate(item);
            item.LastCompleted = DateTimeOffset.Now;

            switch (item.ChildrenType)
            {
                case ToDoItemChildrenType.RequireCompletion:
                    break;
                case ToDoItemChildrenType.IgnoreCompletion:
                    break;
                case ToDoItemChildrenType.CircleCompletion:
                    await CircleCompletionAsync(context, item);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            item.IsCompleted = false;
        }

        await context.SaveChangesAsync();
    }

    private async Task CircleCompletionAsync(SpravyToDoDbContext context, ToDoItemEntity item)
    {
        var children = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var maxOrderIndex = children.Max(x => x.OrderIndex);
        var nextOrderIndex = item.CurrentCircleOrderIndex + 1;

        if (nextOrderIndex > maxOrderIndex)
        {
            nextOrderIndex = 0;
        }

        item.CurrentCircleOrderIndex = nextOrderIndex;

        foreach (var child in children)
        {
            if (child.OrderIndex == nextOrderIndex)
            {
                child.IsCompleted = false;
                child.Type = ToDoItemType.Planned;
                child.DueDate = item.DueDate;
            }
            else
            {
                child.IsCompleted = true;
                child.Type = ToDoItemType.Value;
            }
        }
    }

    private async Task<bool> IsCanFinishToDoItem(SpravyToDoDbContext context, ToDoItemEntity item)
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
            case ToDoItemType.PeriodicityOffset:
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
                ToDoItemType.PeriodicityOffset => item.DueDate,
                _ => throw new ArgumentOutOfRangeException()
            };

            var isCanComplete = await IsCanFinishToDoItem(context, child, dueDate);

            if (!isCanComplete)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> IsCanFinishToDoSubItemPeriodicity(
        SpravyToDoDbContext context,
        ToDoItemEntity item,
        DateTimeOffset? rootDue
    )
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
            var isCanComplete = await IsCanFinishToDoItem(context, child, rootDue);

            if (!isCanComplete)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> IsCanFinishToDoSubItemPeriodicityOffset(
        SpravyToDoDbContext context,
        ToDoItemEntity item,
        DateTimeOffset? rootDue
    )
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
            var isCanComplete = await IsCanFinishToDoItem(context, child, rootDue);

            if (!isCanComplete)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> IsCanFinishToDoSubItemPlanned(
        SpravyToDoDbContext context,
        ToDoItemEntity item,
        DateTimeOffset? rootDue
    )
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
            var isCanComplete = await IsCanFinishToDoItem(context, child, rootDue);

            if (!isCanComplete)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> IsCanFinishToDoSubItemValue(
        SpravyToDoDbContext context,
        ToDoItemEntity item,
        DateTimeOffset? rootDue
    )
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
            var isCanComplete = await IsCanFinishToDoItem(context, child, rootDue);

            if (!isCanComplete)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> IsCanFinishToDoSubItemGroup(
        SpravyToDoDbContext context,
        ToDoItemEntity item,
        DateTimeOffset? rootDue
    )
    {
        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == item.Id)
            .ToArrayAsync();

        foreach (var child in children)
        {
            var isCanComplete = await IsCanFinishToDoItem(context, child, rootDue);

            if (!isCanComplete)
            {
                return false;
            }
        }

        return true;
    }

    private Task<bool> IsCanFinishToDoItem(SpravyToDoDbContext context, ToDoItemEntity item, DateTimeOffset? rootDue)
    {
        switch (item.Type)
        {
            case ToDoItemType.Value: return IsCanFinishToDoSubItemValue(context, item, rootDue);
            case ToDoItemType.Group: return IsCanFinishToDoSubItemGroup(context, item, rootDue);
            case ToDoItemType.Planned: return IsCanFinishToDoSubItemPlanned(context, item, rootDue);
            case ToDoItemType.Periodicity: return IsCanFinishToDoSubItemPeriodicity(context, item, rootDue);
            case ToDoItemType.PeriodicityOffset: return IsCanFinishToDoSubItemPeriodicityOffset(context, item, rootDue);
            default: throw new ArgumentOutOfRangeException();
        }
    }

    public async Task UpdateToDoItemNameAsync(Guid id, string name)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.Name = name;
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemOrderIndexAsync(UpdateOrderIndexToDoItemOptions options)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
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
        await NormalizeOrderIndexAsync(context, item.ParentId);
    }

    public async Task UpdateToDoItemDescriptionAsync(Guid id, string description)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.Description = description;
        await context.SaveChangesAsync();
    }

    public async Task SkipToDoItemAsync(Guid id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        switch (item.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
            {
                var isCanComplete = await IsCanFinishToDoItem(context, item);

                if (!isCanComplete)
                {
                    throw new Exception("Need close sub tasks.");
                }

                break;
            }
            case ToDoItemChildrenType.IgnoreCompletion:
                break;
            case ToDoItemChildrenType.CircleCompletion:
            {
                var isCanComplete = await IsCanFinishToDoItem(context, item);

                if (!isCanComplete)
                {
                    throw new Exception("Need close sub tasks.");
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
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
            case ToDoItemType.PeriodicityOffset:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        item.SkippedCount++;
        UpdateDueDate(item);
        item.LastCompleted = DateTimeOffset.Now;

        switch (item.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
                break;
            case ToDoItemChildrenType.IgnoreCompletion:
                break;
            case ToDoItemChildrenType.CircleCompletion:
                await CircleCompletionAsync(context, item);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await context.SaveChangesAsync();
    }

    public async Task FailToDoItemAsync(Guid id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();


        switch (item.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
            {
                var isCanComplete = await IsCanFinishToDoItem(context, item);

                if (!isCanComplete)
                {
                    throw new Exception("Need close sub tasks.");
                }

                break;
            }
            case ToDoItemChildrenType.IgnoreCompletion:
                break;
            case ToDoItemChildrenType.CircleCompletion:
            {
                var isCanComplete = await IsCanFinishToDoItem(context, item);

                if (!isCanComplete)
                {
                    throw new Exception("Need close sub tasks.");
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
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
            case ToDoItemType.PeriodicityOffset:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        item.FailedCount++;
        UpdateDueDate(item);
        item.LastCompleted = DateTimeOffset.Now;

        switch (item.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
                break;
            case ToDoItemChildrenType.IgnoreCompletion:
                break;
            case ToDoItemChildrenType.CircleCompletion:
                await CircleCompletionAsync(context, item);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<IToDoSubItem>> SearchToDoSubItemsAsync(string searchText)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.Name.Contains(searchText))
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var result = await ConvertAsync(context, items);

        return result;
    }

    public async Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.Type = type;
        await context.SaveChangesAsync();
    }

    public async Task AddCurrentToDoItemAsync(Guid id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.IsCurrent = true;
        await context.SaveChangesAsync();
    }

    public async Task RemoveCurrentToDoItemAsync(Guid id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.IsCurrent = false;
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<IToDoSubItem>> GetCurrentToDoItemsAsync()
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.IsCurrent)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var result = await ConvertAsync(context, items);

        return result;
    }

    public async Task UpdateToDoItemAnnuallyPeriodicityAsync(Guid id, AnnuallyPeriodicity periodicity)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.SetDaysOfYear(periodicity.Days);
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemMonthlyPeriodicityAsync(Guid id, MonthlyPeriodicity periodicity)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.SetDaysOfMonth(periodicity.Days);
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemWeeklyPeriodicityAsync(Guid id, WeeklyPeriodicity periodicity)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.SetDaysOfWeek(periodicity.Days);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<IToDoSubItem>> GetLeafToDoSubItemsAsync(Guid id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

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
            await foreach (var item in GetLeafToDoItemsAsync(context, entity))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public async Task<IEnumerable<ToDoSelectorItem>> GetToDoSelectorItemsAsync(Guid[] ignoreIds)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == null && !ignoreIds.Contains(x.Id))
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var result = new ToDoSelectorItem[items.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var item = items[i];
            var children = await GetToDoSelectorItemsAsync(context, item.Id, ignoreIds);
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

        await using var context = await dbContextFactory.CreateDbContextAsync();
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
        await using var context = await dbContextFactory.CreateDbContextAsync();
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

    public async Task<string> ToDoItemToStringAsync(Guid id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var builder = new StringBuilder();
        await ToDoItemToStringAsync(context, id, 0, builder);

        return builder.ToString();
    }

    public async Task UpdateToDoItemDaysOffsetAsync(Guid id, ushort days)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.DaysOffset = days;
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.MonthsOffset = months;
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.WeeksOffset = weeks;
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemYearsOffsetAsync(Guid id, ushort years)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.YearsOffset = years;
        await context.SaveChangesAsync();
    }

    public async Task UpdateToDoItemChildrenTypeAsync(Guid id, ToDoItemChildrenType type)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();
        item.ChildrenType = type;
        await context.SaveChangesAsync();
    }

    private async Task ToDoItemToStringAsync(SpravyToDoDbContext context, Guid id, ushort level, StringBuilder builder)
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        foreach (var item in items)
        {
            builder.Duplicate(" ", level);
            builder.Append(item.Name);
            builder.AppendLine();
            await ToDoItemToStringAsync(context, item.Id, (ushort)(level + 1), builder);
        }
    }

    private async Task<ToDoSelectorItem[]> GetToDoSelectorItemsAsync(
        SpravyToDoDbContext context,
        Guid id,
        Guid[] ignoreIds
    )
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id && !ignoreIds.Contains(x.Id))
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var result = new ToDoSelectorItem[items.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var item = items[i];
            var children = await GetToDoSelectorItemsAsync(context, item.Id, ignoreIds);
            result[i] = new ToDoSelectorItem(item.Id, item.Name, children);
        }

        return result;
    }

    private async IAsyncEnumerable<IToDoSubItem> GetLeafToDoItemsAsync(
        SpravyToDoDbContext context,
        ToDoItemEntity itemEntity
    )
    {
        var entities = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == itemEntity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        if (entities.IsEmpty())
        {
            yield return await ConvertAsync(context, itemEntity);
            yield break;
        }

        foreach (var entity in entities)
        {
            await foreach (var item in GetLeafToDoItemsAsync(context, entity))
            {
                yield return item;
            }
        }
    }

    private async Task NormalizeOrderIndexAsync(SpravyToDoDbContext context, Guid? parentId)
    {
        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == parentId)
            .ToArrayAsync();

        var ordered = Enumerable.ToArray(items.OrderBy(x => x.OrderIndex));

        for (var index = 0u; index < ordered.LongLength; index++)
        {
            ordered[index].OrderIndex = index;
        }

        await context.SaveChangesAsync();
    }

    private async Task GetParentsAsync(SpravyToDoDbContext context, Guid id, List<ToDoItemParent> parents)
    {
        var parent = await context.Set<ToDoItemEntity>().Include(x => x.Parent).SingleAsync(x => x.Id == id);

        if (parent.Parent is null)
        {
            return;
        }

        parents.Add(mapper.Map<ToDoItemParent>(parent.Parent));
        await GetParentsAsync(context, parent.Parent.Id, parents);
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
            case ToDoItemType.PeriodicityOffset:
                AddPeriodicityOffset(item);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddPeriodicityOffset(ToDoItemEntity item)
    {
        item.DueDate = item.DueDate.AddDays(item.DaysOffset + item.WeeksOffset * 7)
            .AddMonths(item.MonthsOffset)
            .AddYears(item.YearsOffset);
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
                    : item.DueDate.AddDays(7 - (double)dayOfWeek + (double)daysOfWeek.First().ThrowIfNullStruct());

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
                    : item.DueDate.AddMonths(1)
                        .WithDay(Math.Min(daysOfMonth.First().ThrowIfNullStruct(), daysInNextMonth));

                break;
            }
            case TypeOfPeriodicity.Annually:
            {
                var now = item.DueDate;
                var daysOfYear = item.GetDaysOfYear().Order().Select(x => (DayOfYear?)x).ThrowIfEmpty().ToArray();
                var nextDay = daysOfYear.FirstOrDefault(
                    x => x.ThrowIfNullStruct().Month >= now.Month && x.ThrowIfNullStruct().Day > now.Day
                );
                var daysInNextMonth = DateTime.DaysInMonth(now.Year + 1, daysOfYear.First().ThrowIfNullStruct().Month);

                item.DueDate = nextDay is not null
                    ? item.DueDate.WithMonth(nextDay.Value.Month)
                        .WithDay(Math.Min(DateTime.DaysInMonth(now.Year, nextDay.Value.Month), nextDay.Value.Day))
                    : item.DueDate.AddYears(1)
                        .WithMonth(daysOfYear.First().ThrowIfNullStruct().Month)
                        .WithDay(Math.Min(daysInNextMonth, daysOfYear.First().ThrowIfNullStruct().Day));

                break;
            }
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
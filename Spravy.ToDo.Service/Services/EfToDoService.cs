using System.Runtime.CompilerServices;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spravy.Db.Extensions;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Service.Extensions;
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
    private readonly IFactory<SpravyDbToDoDbContext> dbContextFactory;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly GetterToDoItemParametersService getterToDoItemParametersService;

    public EfToDoService(
        IMapper mapper,
        IFactory<SpravyDbToDoDbContext> dbContextFactory,
        IHttpContextAccessor httpContextAccessor,
        GetterToDoItemParametersService getterToDoItemParametersService
    )
    {
        this.mapper = mapper;
        this.dbContextFactory = dbContextFactory;
        this.httpContextAccessor = httpContextAccessor;
        this.getterToDoItemParametersService = getterToDoItemParametersService;
    }

    public async Task UpdateToDoItemDescriptionTypeAsync(
        Guid id,
        DescriptionType type,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.FindAsync<ToDoItemEntity>(id);
                item = item.ThrowIfNull();
                item.DescriptionType = type;
            }
        );
    }

    public async Task ResetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.FindAsync<ToDoItemEntity>(id);
                item = item.ThrowIfNull();
                await CircleCompletionAsync(context, item, cancellationToken);
                await StepCompletionAsync(context, item, cancellationToken);
            }
        );
    }

    public async Task RandomizeChildrenOrderIndexAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var children = await c.Set<ToDoItemEntity>()
                    .Where(x => x.ParentId == id)
                    .ToArrayAsync(cancellationToken);

                children.Randomize();

                for (var i = children.Length - 1; i > 0; i--)
                {
                    children[i].OrderIndex = (uint)i;
                }
            }
        );
    }

    public async Task<IEnumerable<ToDoShortItem>> GetParentsAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<ToDoItemEntity>(id);
        item = item.ThrowIfNull();

        var parents = new List<ToDoShortItem>
        {
            new(item.Id, item.Name)
        };

        await GetParentsAsync(context, id, parents, cancellationToken);
        parents.Reverse();

        return parents;
    }

    public async Task<IEnumerable<Guid>> SearchToDoItemIdsAsync(string searchText, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.Name.Contains(searchText))
            .OrderBy(x => x.OrderIndex)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return items;
    }

    public async Task<IEnumerable<Guid>> GetLeafToDoItemIdsAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        var entities = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        if (entities.IsEmpty())
        {
            return Enumerable.Empty<Guid>();
        }

        var result = new List<Guid>();

        foreach (var entity in entities)
        {
            await foreach (var item in GetLeafToDoItemIdsAsync(context, entity, cancellationToken))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public async Task<ToDoItem> GetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        var parameters = await getterToDoItemParametersService.GetToDoItemParametersAsync(
            context,
            item,
            httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset(),
            cancellationToken
        );

        var result = mapper.Map<ToDoItem>(item, a => a.Items.Add(SpravyToDoDbProfile.ParametersName, parameters));

        return result;
    }

    public async Task<IEnumerable<Guid>> GetChildrenToDoItemIdsAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        var ids = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id)
            .OrderBy(x => x.OrderIndex)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return ids;
    }

    public async Task<IEnumerable<ToDoShortItem>> GetChildrenToDoItemShortsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        return mapper.Map<IEnumerable<ToDoShortItem>>(items);
    }

    public async Task<IEnumerable<Guid>> GetRootToDoItemIdsAsync(CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        var ids = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.OrderIndex)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return ids;
    }

    public async Task<IEnumerable<Guid>> GetFavoriteToDoItemIdsAsync(CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        var ids = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .OrderBy(x => x.OrderIndex)
            .Where(x => x.IsFavorite)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return ids;
    }

    public async Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var id = Guid.NewGuid();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var items = await c.Set<ToDoItemEntity>()
                    .AsNoTracking()
                    .Where(x => x.ParentId == null)
                    .Select(x => x.OrderIndex)
                    .ToArrayAsync(cancellationToken);

                var newEntity = mapper.Map<ToDoItemEntity>(options);
                newEntity.Id = id;
                newEntity.OrderIndex = items.Length == 0 ? 0 : items.Max() + 1;
                newEntity.Description = options.Description;
                newEntity.DescriptionType = options.DescriptionType;
                newEntity.Link = mapper.Map<string>(options.Link);
                await c.Set<ToDoItemEntity>().AddAsync(newEntity, cancellationToken);
            }
        );

        return id;
    }

    public async Task<Guid> AddToDoItemAsync(AddToDoItemOptions options, CancellationToken cancellationToken)
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();
        await using var context = dbContextFactory.Create();
        var id = Guid.NewGuid();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var parent = await c.Set<ToDoItemEntity>().FindAsync(options.ParentId);
                parent = parent.ThrowIfNull();

                var items = await c.Set<ToDoItemEntity>()
                    .AsNoTracking()
                    .Where(x => x.ParentId == options.ParentId)
                    .Select(x => x.OrderIndex)
                    .ToArrayAsync(cancellationToken);

                var toDoItem = mapper.Map<ToDoItemEntity>(options);
                toDoItem.Description = options.Description;
                toDoItem.Id = id;
                toDoItem.OrderIndex = items.Length == 0 ? 0 : items.Max() + 1;

                toDoItem.DueDate =
                    parent.DueDate < DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly()
                        ? DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly()
                        : parent.DueDate;

                toDoItem.TypeOfPeriodicity = parent.TypeOfPeriodicity;
                toDoItem.DaysOfMonth = parent.DaysOfMonth;
                toDoItem.DaysOfWeek = parent.DaysOfWeek;
                toDoItem.DaysOfYear = parent.DaysOfYear;
                toDoItem.WeeksOffset = parent.WeeksOffset;
                toDoItem.DaysOffset = parent.DaysOffset;
                toDoItem.MonthsOffset = parent.MonthsOffset;
                toDoItem.YearsOffset = parent.YearsOffset;
                toDoItem.Link = options.Link?.AbsoluteUri ?? string.Empty;
                toDoItem.DescriptionType = options.DescriptionType;
                await c.Set<ToDoItemEntity>().AddAsync(toDoItem, cancellationToken);
            }
        );

        return id;
    }

    public async Task DeleteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();

                var children = await c.Set<ToDoItemEntity>()
                    .AsNoTracking()
                    .Where(x => x.ParentId == id)
                    .ToArrayAsync(cancellationToken);

                foreach (var child in children)
                {
                    await DeleteToDoItemAsync(child.Id, context, cancellationToken);
                }

                c.Set<ToDoItemEntity>().Remove(item);
                await NormalizeOrderIndexAsync(c, item.ParentId, cancellationToken);
            }
        );
    }

    private async Task DeleteToDoItemAsync(Guid id, SpravyDbToDoDbContext context, CancellationToken cancellationToken)
    {
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        var children = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id)
            .ToArrayAsync(cancellationToken);

        foreach (var child in children)
        {
            await DeleteToDoItemAsync(child.Id, context, cancellationToken);
        }

        context.Set<ToDoItemEntity>().Remove(item);
    }

    public async Task UpdateToDoItemTypeOfPeriodicityAsync(
        Guid id,
        TypeOfPeriodicity type,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.TypeOfPeriodicity = type;
            }
        );
    }

    public async Task UpdateToDoItemDueDateAsync(Guid id, DateOnly dueDate, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.DueDate = dueDate;
            }
        );
    }

    public async Task UpdateToDoItemCompleteStatusAsync(Guid id, bool isComplete, CancellationToken cancellationToken)
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();

                if (isComplete)
                {
                    var parameters =
                        await getterToDoItemParametersService.GetToDoItemParametersAsync(
                            context,
                            item,
                            offset,
                            cancellationToken
                        );

                    if (!parameters.IsCan.HasFlag(ToDoItemIsCan.CanComplete))
                    {
                        throw new ArgumentException();
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
                        case ToDoItemType.Circle:
                            item.IsCompleted = true;

                            break;
                        case ToDoItemType.Step:
                            item.IsCompleted = true;

                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }

                    UpdateDueDate(item, offset, cancellationToken);
                    item.LastCompleted = DateTimeOffset.Now;
                    await CircleCompletionAsync(context, item, cancellationToken);
                    await StepCompletionAsync(context, item, cancellationToken);
                }
                else
                {
                    item.IsCompleted = false;
                }
            }
        );
    }

    private async Task StepCompletionAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity item,
        CancellationToken cancellationToken
    )
    {
        var steps = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Step)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        foreach (var step in steps)
        {
            step.IsCompleted = false;
        }

        var groups = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Group)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        foreach (var group in groups)
        {
            await StepCompletionAsync(context, group, cancellationToken);
        }
    }

    private async Task CircleCompletionAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity item,
        CancellationToken cancellationToken
    )
    {
        var children = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Circle)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        var childrenOrderIndexCount = children.DistinctBy(x => x.OrderIndex).Count();

        if (childrenOrderIndexCount != children.Length)
        {
            await NormalizeOrderIndexAsync(context, item.Id, cancellationToken);

            children = await context.Set<ToDoItemEntity>()
                .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Circle)
                .OrderBy(x => x.OrderIndex)
                .ToArrayAsync(cancellationToken);
        }

        if (children.Length != 0)
        {
            var next = children.FirstOrDefault(x => x.OrderIndex > item.CurrentCircleOrderIndex);
            var nextOrderIndex = next?.OrderIndex ?? children.First().OrderIndex;
            item.CurrentCircleOrderIndex = nextOrderIndex;

            foreach (var child in children)
            {
                child.IsCompleted = child.OrderIndex != nextOrderIndex;
            }
        }

        var groups = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Group)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        foreach (var group in groups)
        {
            await CircleCompletionAsync(context, group, cancellationToken);
        }
    }

    private async Task CircleSkipAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity item,
        CancellationToken cancellationToken
    )
    {
        var children = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Circle)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        if (children.Length != 0)
        {
            var next = children.SingleOrDefault(x => x.OrderIndex == item.CurrentCircleOrderIndex)
                       ?? children.FirstOrDefault(x => x.OrderIndex > item.CurrentCircleOrderIndex);

            var nextOrderIndex = next?.OrderIndex ?? children.First().OrderIndex;
            item.CurrentCircleOrderIndex = nextOrderIndex;

            foreach (var child in children)
            {
                child.IsCompleted = child.OrderIndex != nextOrderIndex;
            }
        }

        var groups = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Group)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        foreach (var group in groups)
        {
            await CircleCompletionAsync(context, group, cancellationToken);
        }
    }

    public async Task UpdateToDoItemNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.Name = name;
            }
        );
    }

    public async Task UpdateToDoItemOrderIndexAsync(
        UpdateOrderIndexToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(options.Id);
                item = item.ThrowIfNull();
                var targetItem = await c.Set<ToDoItemEntity>().FindAsync(options.TargetId);
                targetItem = targetItem.ThrowIfNull();
                var orderIndex = options.IsAfter ? targetItem.OrderIndex + 1 : targetItem.OrderIndex;

                var items = await c.Set<ToDoItemEntity>()
                    .Where(
                        x => x.ParentId == item.ParentId
                             && x.Id != item.Id
                             && x.OrderIndex >= orderIndex
                    )
                    .ToArrayAsync(cancellationToken);

                foreach (var itemEntity in items)
                {
                    itemEntity.OrderIndex++;
                }

                item.OrderIndex = orderIndex;
                await c.SaveChangesAsync(cancellationToken);
                await NormalizeOrderIndexAsync(c, item.ParentId, cancellationToken);
            }
        );
    }

    public async Task UpdateToDoItemDescriptionAsync(Guid id, string description, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.Description = description;
            }
        );
    }

    public async Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.Type = type;
            }
        );
    }

    public async Task AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.IsFavorite = true;
            }
        );
    }

    public async Task RemoveFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.IsFavorite = false;
            }
        );
    }

    public async Task UpdateToDoItemIsRequiredCompleteInDueDateAsync(
        Guid id,
        bool value,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.IsRequiredCompleteInDueDate = value;
            }
        );
    }

    public async Task<IEnumerable<Guid>> GetTodayToDoItemsAsync(CancellationToken cancellationToken)
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();
        await using var context = dbContextFactory.Create();

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(
                x => !x.IsCompleted
                     && (x.Type == ToDoItemType.Periodicity
                         || x.Type == ToDoItemType.PeriodicityOffset
                         || x.Type == ToDoItemType.Planned)
            )
            .Select(
                x => new
                {
                    x.Id,
                    x.DueDate,
                }
            )
            .ToArrayAsync(cancellationToken);

        return items.Where(x => x.DueDate <= DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
            .Select(x => x.Id)
            .ToArray();
    }

    public async Task UpdateToDoItemAnnuallyPeriodicityAsync(
        Guid id,
        AnnuallyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.SetDaysOfYear(periodicity.Days);
            }
        );
    }

    public async Task UpdateToDoItemMonthlyPeriodicityAsync(
        Guid id,
        MonthlyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.SetDaysOfMonth(periodicity.Days);
            }
        );
    }

    public async Task UpdateToDoItemWeeklyPeriodicityAsync(
        Guid id,
        WeeklyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.SetDaysOfWeek(periodicity.Days);
            }
        );
    }

    public async Task<IEnumerable<ToDoSelectorItem>> GetToDoSelectorItemsAsync(
        Guid[] ignoreIds,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == null && !ignoreIds.Contains(x.Id))
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        var result = new ToDoSelectorItem[items.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var item = items[i];
            var children = await GetToDoSelectorItemsAsync(context, item.Id, ignoreIds, cancellationToken);
            result[i] = new(item.Id, item.Name, children);
        }

        return result;
    }

    public async Task UpdateToDoItemParentAsync(Guid id, Guid parentId, CancellationToken cancellationToken)
    {
        if (id == parentId)
        {
            throw new();
        }

        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var entity = await c.Set<ToDoItemEntity>().FindAsync(id);

                var items = await c.Set<ToDoItemEntity>()
                    .AsNoTracking()
                    .Where(x => x.ParentId == parentId)
                    .Select(x => x.OrderIndex)
                    .ToArrayAsync();

                entity = entity.ThrowIfNull();
                entity.ParentId = parentId;
                entity.OrderIndex = items.Length == 0 ? 0 : items.Max() + 1;
            }
        );
    }

    public async Task ToDoItemToRootAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var entity = await c.Set<ToDoItemEntity>().FindAsync(id);

                var items = await c.Set<ToDoItemEntity>()
                    .AsNoTracking()
                    .Where(x => x.ParentId == null)
                    .Select(x => x.OrderIndex)
                    .ToArrayAsync();

                entity = entity.ThrowIfNull();
                entity.ParentId = null;
                entity.OrderIndex = items.Length == 0 ? 0 : items.Max() + 1;
            }
        );
    }

    public async Task<string> ToDoItemToStringAsync(
        ToDoItemToStringOptions options,
        CancellationToken cancellationToken
    )
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();
        await using var context = dbContextFactory.Create();
        var builder = new StringBuilder();
        await ToDoItemToStringAsync(context, options, 0, builder, offset, cancellationToken);

        return builder.ToString();
    }

    public async Task UpdateToDoItemDaysOffsetAsync(Guid id, ushort days, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.DaysOffset = days;
            }
        );
    }

    public async Task UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.MonthsOffset = months;
            }
        );
    }

    public async Task UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.WeeksOffset = weeks;
            }
        );
    }

    public async Task UpdateToDoItemYearsOffsetAsync(Guid id, ushort years, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.YearsOffset = years;
            }
        );
    }

    public async Task UpdateToDoItemChildrenTypeAsync(
        Guid id,
        ToDoItemChildrenType type,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.ChildrenType = type;
            }
        );
    }

    public async Task<IEnumerable<ToDoShortItem>> GetSiblingsAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<ToDoItemEntity>(id);
        item = item.ThrowIfNull();

        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.ParentId && x.Id != item.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        return mapper.Map<IEnumerable<ToDoShortItem>>(items);
    }

    public async Task<ActiveToDoItem?> GetCurrentActiveToDoItemAsync(CancellationToken cancellationToken)
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();
        await using var context = dbContextFactory.Create();

        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        foreach (var item in items)
        {
            var parameters =
                await getterToDoItemParametersService.GetToDoItemParametersAsync(
                    context,
                    item,
                    offset,
                    cancellationToken
                );

            if (parameters.ActiveItem is not null)
            {
                return parameters.ActiveItem;
            }

            switch (parameters.Status)
            {
                case ToDoItemStatus.Miss:
                    return null;
                case ToDoItemStatus.ReadyForComplete:
                    return null;
                case ToDoItemStatus.Planned:
                    break;
                case ToDoItemStatus.Completed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return null;
    }

    public async Task UpdateToDoItemLinkAsync(Guid id, Uri? link, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var value = await c.FindAsync<ToDoItemEntity>(id);
                value = value.ThrowIfNull();
                value.Link = mapper.Map<string>(link) ?? string.Empty;
            }
        );
    }

    public async Task<PlannedToDoItemSettings> GetPlannedToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<PlannedToDoItemSettings>(item);
    }

    public async Task<ValueToDoItemSettings> GetValueToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<ValueToDoItemSettings>(item);
    }

    public async Task<PeriodicityToDoItemSettings> GetPeriodicityToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<PeriodicityToDoItemSettings>(item);
    }

    public async Task<WeeklyPeriodicity> GetWeeklyPeriodicityAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<WeeklyPeriodicity>(item);
    }

    public async Task<MonthlyPeriodicity> GetMonthlyPeriodicityAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<MonthlyPeriodicity>(item);
    }

    public async Task<AnnuallyPeriodicity> GetAnnuallyPeriodicityAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<AnnuallyPeriodicity>(item);
    }

    public async Task<PeriodicityOffsetToDoItemSettings> GetPeriodicityOffsetToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<PeriodicityOffsetToDoItemSettings>(item);
    }

    public async IAsyncEnumerable<IEnumerable<ToDoItem>> GetToDoItemsAsync(
        Guid[] ids,
        uint chunkSize,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        var items = new List<ToDoItem>();

        for (var i = 0; i < ids.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (i % chunkSize == 0)
            {
                if (items.Count > 0)
                {
                    yield return items.ToArray();

                    items.Clear();
                }
            }

            items.Add(await GetToDoItemAsync(ids[i], cancellationToken));
        }

        if (items.Count > 0)
        {
            yield return items.ToArray();

            items.Clear();
        }
    }

    private async Task ToDoItemToStringAsync(
        SpravyDbToDoDbContext context,
        ToDoItemToStringOptions options,
        ushort level,
        StringBuilder builder,
        TimeSpan offset,
        CancellationToken cancellationToken
    )
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == options.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        foreach (var item in items)
        {
            var parameters = await getterToDoItemParametersService.GetToDoItemParametersAsync(
                context,
                item,
                offset,
                cancellationToken
            );

            if (!options.Statuses.Span.ToArray().Contains(parameters.Status))
            {
                continue;
            }

            builder.Duplicate(" ", level);
            builder.Append(item.Name);
            builder.AppendLine();

            await ToDoItemToStringAsync(
                context,
                new(options.Statuses.ToArray(), item.Id),
                (ushort)(level + 1),
                builder,
                offset,
                cancellationToken
            );
        }
    }

    private async Task<ToDoSelectorItem[]> GetToDoSelectorItemsAsync(
        SpravyDbToDoDbContext context,
        Guid id,
        Guid[] ignoreIds,
        CancellationToken cancellationToken
    )
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id && !ignoreIds.Contains(x.Id))
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        var result = new ToDoSelectorItem[items.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var item = items[i];
            var children = await GetToDoSelectorItemsAsync(context, item.Id, ignoreIds, cancellationToken);
            result[i] = new(item.Id, item.Name, children);
        }

        return result;
    }

    private async IAsyncEnumerable<Guid> GetLeafToDoItemIdsAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity itemEntity,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        var entities = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == itemEntity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        if (entities.IsEmpty())
        {
            yield return itemEntity.Id;

            yield break;
        }

        foreach (var entity in entities)
        {
            await foreach (var item in GetLeafToDoItemIdsAsync(context, entity, cancellationToken))
            {
                yield return item;
            }
        }
    }

    private async Task NormalizeOrderIndexAsync(
        SpravyDbToDoDbContext context,
        Guid? parentId,
        CancellationToken cancellationToken
    )
    {
        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == parentId)
            .ToArrayAsync(cancellationToken);

        var ordered = Enumerable.ToArray(items.OrderBy(x => x.OrderIndex));

        for (var index = 0u; index < ordered.LongLength; index++)
        {
            ordered[index].OrderIndex = index;
        }
    }

    private async Task GetParentsAsync(
        SpravyDbToDoDbContext context,
        Guid id,
        List<ToDoShortItem> parents,
        CancellationToken cancellationToken
    )
    {
        var parent = await context.Set<ToDoItemEntity>()
            .Include(x => x.Parent)
            .SingleAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (parent.Parent is null)
        {
            return;
        }

        parents.Add(mapper.Map<ToDoShortItem>(parent.Parent));
        await GetParentsAsync(context, parent.Parent.Id, parents, cancellationToken);
    }

    private void UpdateDueDate(ToDoItemEntity item, TimeSpan offset, CancellationToken cancellationToken)
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
                AddPeriodicity(item, cancellationToken);

                break;
            case ToDoItemType.PeriodicityOffset:
                AddPeriodicityOffset(item, offset, cancellationToken);

                break;
            case ToDoItemType.Circle:
                break;
            case ToDoItemType.Step:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddPeriodicityOffset(ToDoItemEntity item, TimeSpan offset, CancellationToken cancellationToken)
    {
        if (item.IsRequiredCompleteInDueDate)
        {
            item.DueDate = item.DueDate
                .AddDays(item.DaysOffset + item.WeeksOffset * 7)
                .AddMonths(item.MonthsOffset)
                .AddYears(item.YearsOffset);
        }
        else
        {
            item.DueDate = DateTimeOffset.UtcNow.Add(offset)
                .Date.ToDateOnly()
                .AddDays(item.DaysOffset + item.WeeksOffset * 7)
                .AddMonths(item.MonthsOffset)
                .AddYears(item.YearsOffset);
        }
    }

    private void AddPeriodicity(ToDoItemEntity item, CancellationToken cancellationToken)
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
                    ? item.DueDate.AddDays((int)nextDay - (int)dayOfWeek)
                    : item.DueDate.AddDays(7 - (int)dayOfWeek + (int)daysOfWeek.First().ThrowIfNullStruct());

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
                    : item.DueDate
                        .AddMonths(1)
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
                    ? item.DueDate
                        .WithMonth(nextDay.Value.Month)
                        .WithDay(Math.Min(DateTime.DaysInMonth(now.Year, nextDay.Value.Month), nextDay.Value.Day))
                    : item.DueDate
                        .AddYears(1)
                        .WithMonth(daysOfYear.First().ThrowIfNullStruct().Month)
                        .WithDay(Math.Min(daysInNextMonth, daysOfYear.First().ThrowIfNullStruct().Day));

                break;
            }
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
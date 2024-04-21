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
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Extensions;
using Spravy.ToDo.Db.Mapper.Profiles;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Db.Services;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Errors;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;

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

    public ConfiguredValueTaskAwaitable<Result> CloneToDoItemAsync(
        Guid cloneId,
        Guid? parentId,
        CancellationToken cancellationToken
    )
    {
        return CloneToDoItemCore(cloneId, parentId, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> CloneToDoItemCore(Guid cloneId, Guid? parentId, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        var clone = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .SingleAsync(x => x.Id == cloneId, cancellationToken);

        await context.ExecuteSaveChangesTransactionAsync(
            async c => await AddCloneAsync(c, clone, parentId, cancellationToken),
            cancellationToken
        );

        return Result.Success;
    }

    private async ValueTask<Result> AddCloneAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity clone,
        Guid? parentId,
        CancellationToken cancellationToken
    )
    {
        var id = clone.Id;
        clone.Id = Guid.NewGuid();
        clone.ParentId = parentId;
        await context.AddAsync(clone, cancellationToken);

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id)
            .ToArrayAsync(cancellationToken);

        foreach (var item in items)
        {
            await AddCloneAsync(context, item, clone.Id, cancellationToken);
        }

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDescriptionTypeAsync(
        Guid id,
        DescriptionType type,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemDescriptionTypeCore(id, type, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> UpdateToDoItemDescriptionTypeCore(
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
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> ResetToDoItemAsync(
        ResetToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return ResetToDoItemCore(options, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> ResetToDoItemCore(ResetToDoItemOptions options, CancellationToken cancellationToken)
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.FindAsync<ToDoItemEntity>(options.Id);
                item = item.ThrowIfNull();

                if (options.IsCompleteCurrentTask)
                {
                    item.IsCompleted = true;
                    UpdateDueDate(item, offset, cancellationToken);
                }

                await CircleCompletionAsync(
                    context,
                    item,
                    options.IsMoveCircleOrderIndex,
                    options.IsCompleteChildrenTask,
                    options.IsOnlyCompletedTasks,
                    cancellationToken
                );

                await StepCompletionAsync(context, item, options.IsCompleteChildrenTask, cancellationToken);
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> RandomizeChildrenOrderIndexAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return RandomizeChildrenOrderIndexCore(id, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> RandomizeChildrenOrderIndexCore(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var children = await c.Set<ToDoItemEntity>()
                    .Where(x => x.ParentId == id)
                    .ToArrayAsync(cancellationToken);

                children.Randomize();

                for (var i = children.Length - 1; i > 0; i--) children[i].OrderIndex = (uint)i;
            },
            cancellationToken
        );

        return Result.Success;
    }


    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GetParentsCore(id, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsCore(
        Guid id,
        CancellationToken cancellationToken
    )
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

        return parents.ToArray().ToReadOnlyMemory().ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsAsync(
        string searchText,
        CancellationToken cancellationToken
    )
    {
        return SearchToDoItemIdsCore(searchText, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsCore(
        string searchText,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.Name.Contains(searchText))
            .OrderBy(x => x.OrderIndex)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return items.ToReadOnlyMemory().ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GetLeafToDoItemIdsCore(id, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsCore(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        var entities = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        if (entities.IsEmpty())
        {
            return ReadOnlyMemory<Guid>.Empty.ToResult();
        }

        var result = new List<Guid>();

        foreach (var entity in entities)
        {
            await foreach (var item in GetLeafToDoItemIdsAsync(context, entity, cancellationToken))
            {
                result.Add(item);
            }
        }

        return result.ToArray().ToReadOnlyMemory().ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<ToDoItem>> GetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return GetToDoItemCore(id, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<ToDoItem>> GetToDoItemCore(Guid id, CancellationToken cancellationToken)
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

        return result.ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GetChildrenToDoItemIdsCore(id, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsCore(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        var ids = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id)
            .OrderBy(x => x.OrderIndex)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return ids.ToReadOnlyMemory().ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetChildrenToDoItemShortsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GetChildrenToDoItemShortsCore(id, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<ReadOnlyMemory<ToDoShortItem>>> GetChildrenToDoItemShortsCore(
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

        return mapper.Map<ToDoShortItem>(items).ToReadOnlyMemory().ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetRootToDoItemIdsAsync(
        CancellationToken cancellationToken
    )
    {
        return GetRootToDoItemIdsCore(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<ReadOnlyMemory<Guid>>> GetRootToDoItemIdsCore(CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        var ids = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.OrderIndex)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return ids.ToReadOnlyMemory().ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(
        CancellationToken cancellationToken
    )
    {
        return GetFavoriteToDoItemIdsCore(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsCore(
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        var ids = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .OrderBy(x => x.OrderIndex)
            .Where(x => x.IsFavorite)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return ids.ToReadOnlyMemory().ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<Guid>> AddRootToDoItemAsync(
        AddRootToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return AddRootToDoItemCore(options, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<Guid>> AddRootToDoItemCore(
        AddRootToDoItemOptions options,
        CancellationToken cancellationToken
    )
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
            },
            cancellationToken
        );

        return id.ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<Guid>> AddToDoItemAsync(
        AddToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return AddToDoItemCore(options, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<Guid>> AddToDoItemCore(
        AddToDoItemOptions options,
        CancellationToken cancellationToken
    )
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
            },
            cancellationToken
        );

        return id.ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return DeleteToDoItemCore(id, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> DeleteToDoItemCore(Guid id, CancellationToken cancellationToken)
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
            },
            cancellationToken
        );

        return Result.Success;
    }

    private async ValueTask<Result> DeleteToDoItemAsync(
        Guid id,
        SpravyDbToDoDbContext context,
        CancellationToken cancellationToken
    )
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

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemTypeOfPeriodicityAsync(
        Guid id,
        TypeOfPeriodicity type,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemTypeOfPeriodicityCore(id, type, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdateToDoItemTypeOfPeriodicityCore(
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
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDueDateAsync(
        Guid id,
        DateOnly dueDate,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemDueDateCore(id, dueDate, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdateToDoItemDueDateCore(
        Guid id,
        DateOnly dueDate,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.DueDate = dueDate;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemCompleteStatusAsync(
        Guid id,
        bool isComplete,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemCompleteStatusCore(id, isComplete, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> UpdateToDoItemCompleteStatusCore(
        Guid id,
        bool isComplete,
        CancellationToken cancellationToken
    )
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();
        await using var context = dbContextFactory.Create();

       return await context.ExecuteSaveChangesTransactionAsync(
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
                        return new Result(new ToDoItemAlreadyCompleteError(item.Id, item.Name));
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
                    await CircleCompletionAsync(context, item, true, false, false, cancellationToken);
                    await StepCompletionAsync(context, item, false, cancellationToken);
                }
                else
                {
                    item.IsCompleted = false;
                }

                return Result.Success;
            },
            cancellationToken
        );
    }

    private async ValueTask<Result> StepCompletionAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity item,
        bool completeTask,
        CancellationToken cancellationToken
    )
    {
        var steps = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Step)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        foreach (var step in steps)
        {
            step.IsCompleted = completeTask;
        }

        var groups = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Group)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        foreach (var group in groups)
        {
            await StepCompletionAsync(context, group, completeTask, cancellationToken);
        }

        return Result.Success;
    }

    private async ValueTask<Result> CircleCompletionAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity item,
        bool moveCircleOrderIndex,
        bool completeTask,
        bool onlyCompletedTasks,
        CancellationToken cancellationToken
    )
    {
        var circleChildren = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Circle)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        var childrenOrderIndexCount = circleChildren.DistinctBy(x => x.OrderIndex).Count();

        if (childrenOrderIndexCount != circleChildren.Length)
        {
            await NormalizeOrderIndexAsync(context, item.Id, cancellationToken);

            circleChildren = await context.Set<ToDoItemEntity>()
                .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Circle)
                .OrderBy(x => x.OrderIndex)
                .ToArrayAsync(cancellationToken);
        }

        if (circleChildren.Length != 0)
        {
            if (!onlyCompletedTasks || circleChildren.All(x => x.IsCompleted))
            {
                var nextOrderIndex = item.CurrentCircleOrderIndex;

                if (moveCircleOrderIndex)
                {
                    var next = circleChildren.FirstOrDefault(x => x.OrderIndex > item.CurrentCircleOrderIndex);
                    nextOrderIndex = next?.OrderIndex ?? circleChildren.First().OrderIndex;
                    item.CurrentCircleOrderIndex = nextOrderIndex;
                }

                foreach (var child in circleChildren)
                {
                    if (completeTask)
                    {
                        child.IsCompleted = true;
                    }
                    else
                    {
                        child.IsCompleted = child.OrderIndex != nextOrderIndex;
                    }
                }
            }
        }

        var groups = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Group)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        foreach (var group in groups)
        {
            await CircleCompletionAsync(context, group, moveCircleOrderIndex, completeTask, onlyCompletedTasks,
                cancellationToken);
        }

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemNameAsync(
        Guid id,
        string name,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemNameCore(id, name, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdateToDoItemNameCore(Guid id, string name, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.Name = name;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemOrderIndexAsync(
        UpdateOrderIndexToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemOrderIndexCore(options, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdateToDoItemOrderIndexCore(
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
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDescriptionAsync(
        Guid id,
        string description,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemDescriptionCore(id, description, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdateToDoItemDescriptionCore(
        Guid id,
        string description,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.Description = description;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemTypeAsync(
        Guid id,
        ToDoItemType type,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemTypeCore(id, type, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdateToDoItemTypeCore(
        Guid id,
        ToDoItemType type,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.Type = type;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return AddFavoriteToDoItemCore(id, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> AddFavoriteToDoItemCore(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.IsFavorite = true;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> RemoveFavoriteToDoItemAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return RemoveFavoriteToDoItemCore(id, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> RemoveFavoriteToDoItemCore(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.IsFavorite = false;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemIsRequiredCompleteInDueDateAsync(
        Guid id,
        bool value,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemIsRequiredCompleteInDueDateCore(id, value, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdateToDoItemIsRequiredCompleteInDueDateCore(
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
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(
        CancellationToken cancellationToken
    )
    {
        return GetTodayToDoItemsCore(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsCore(CancellationToken cancellationToken)
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
                    x.DueDate
                }
            )
            .ToArrayAsync(cancellationToken);

        return items.Where(x => x.DueDate <= DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
            .Select(x => x.Id)
            .ToArray()
            .ToReadOnlyMemory()
            .ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemAnnuallyPeriodicityAsync(
        Guid id,
        AnnuallyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemAnnuallyPeriodicityCore(id, periodicity, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> UpdateToDoItemAnnuallyPeriodicityCore(
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
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemMonthlyPeriodicityAsync(
        Guid id,
        MonthlyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemMonthlyPeriodicityCore(id, periodicity, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> UpdateToDoItemMonthlyPeriodicityCore(
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
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemWeeklyPeriodicityAsync(
        Guid id,
        WeeklyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemWeeklyPeriodicityCore(id, periodicity, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> UpdateToDoItemWeeklyPeriodicityCore(
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
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoSelectorItem>>> GetToDoSelectorItemsAsync(
        Guid[] ignoreIds,
        CancellationToken cancellationToken
    )
    {
        return GetToDoSelectorItemsCore(ignoreIds, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<ReadOnlyMemory<ToDoSelectorItem>>> GetToDoSelectorItemsCore(
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
            result[i] = new ToDoSelectorItem(item.Id, item.Name, children);
        }

        return result.ToReadOnlyMemory().ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemParentAsync(
        Guid id,
        Guid parentId,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemParentCore(id, parentId, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> UpdateToDoItemParentCore(
        Guid id,
        Guid parentId,
        CancellationToken cancellationToken
    )
    {
        if (id == parentId) throw new Exception();

        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var entity = await c.Set<ToDoItemEntity>().FindAsync(id);

                var items = await c.Set<ToDoItemEntity>()
                    .AsNoTracking()
                    .Where(x => x.ParentId == parentId)
                    .Select(x => x.OrderIndex)
                    .ToArrayAsync(cancellationToken);

                entity = entity.ThrowIfNull();
                entity.ParentId = parentId;
                entity.OrderIndex = items.Length == 0 ? 0 : items.Max() + 1;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> ToDoItemToRootAsync(Guid id, CancellationToken cancellationToken)
    {
        return ToDoItemToRootCore(id, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> ToDoItemToRootCore(Guid id, CancellationToken cancellationToken)
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
                    .ToArrayAsync(cancellationToken);

                entity = entity.ThrowIfNull();
                entity.ParentId = null;
                entity.OrderIndex = items.Length == 0 ? 0 : items.Max() + 1;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result<string>> ToDoItemToStringAsync(
        ToDoItemToStringOptions options,
        CancellationToken cancellationToken
    )
    {
        return ToDoItemToStringCore(options, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<string>> ToDoItemToStringCore(
        ToDoItemToStringOptions options,
        CancellationToken cancellationToken
    )
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();
        await using var context = dbContextFactory.Create();
        var builder = new StringBuilder();
        await ToDoItemToStringAsync(context, options, 0, builder, offset, cancellationToken);

        return builder.ToString().ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDaysOffsetAsync(
        Guid id,
        ushort days,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemDaysOffsetCore(id, days, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> UpdateToDoItemDaysOffsetCore(
        Guid id,
        ushort days,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.DaysOffset = days;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemMonthsOffsetAsync(
        Guid id,
        ushort months,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemMonthsOffsetCore(id, months, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> UpdateToDoItemMonthsOffsetCore(
        Guid id,
        ushort months,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.MonthsOffset = months;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemWeeksOffsetAsync(
        Guid id,
        ushort weeks,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemWeeksOffsetCore(id, weeks, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> UpdateToDoItemWeeksOffsetCore(
        Guid id,
        ushort weeks,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.WeeksOffset = weeks;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemYearsOffsetAsync(
        Guid id,
        ushort years,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemYearsOffsetCore(id, years, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdateToDoItemYearsOffsetCore(
        Guid id,
        ushort years,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.Set<ToDoItemEntity>().FindAsync(id);
                item = item.ThrowIfNull();
                item.YearsOffset = years;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemChildrenTypeAsync(
        Guid id,
        ToDoItemChildrenType type,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemChildrenTypeCore(id, type, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> UpdateToDoItemChildrenTypeCore(
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
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetSiblingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GetSiblingsCore(id, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<ReadOnlyMemory<ToDoShortItem>>> GetSiblingsCore(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<ToDoItemEntity>(id);
        item = item.ThrowIfNull();

        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.ParentId && x.Id != item.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        return mapper.Map<ToDoShortItem[]>(items).ToReadOnlyMemory().ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<ActiveToDoItem?>> GetCurrentActiveToDoItemAsync(
        CancellationToken cancellationToken
    )
    {
        return GetCurrentActiveToDoItemCore(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<ActiveToDoItem?>> GetCurrentActiveToDoItemCore(CancellationToken cancellationToken)
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

            if (parameters.ActiveItem is not null) return parameters.ActiveItem.ToResult();

            switch (parameters.Status)
            {
                case ToDoItemStatus.Miss:
                    return new Result<ActiveToDoItem?>((ActiveToDoItem?)null);
                case ToDoItemStatus.ReadyForComplete:
                    return new Result<ActiveToDoItem?>((ActiveToDoItem?)null);
                case ToDoItemStatus.Planned:
                    break;
                case ToDoItemStatus.Completed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return new Result<ActiveToDoItem?>((ActiveToDoItem?)null);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemLinkAsync(
        Guid id,
        Uri? link,
        CancellationToken cancellationToken
    )
    {
        return UpdateToDoItemLinkCore(id, link, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> UpdateToDoItemLinkCore(Guid id, Uri? link, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var value = await c.FindAsync<ToDoItemEntity>(id);
                value = value.ThrowIfNull();
                value.Link = mapper.Map<string>(link) ?? string.Empty;
            },
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result<PlannedToDoItemSettings>> GetPlannedToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GetPlannedToDoItemSettingsCore(id, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<PlannedToDoItemSettings>> GetPlannedToDoItemSettingsCore(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<PlannedToDoItemSettings>(item).ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<ValueToDoItemSettings>> GetValueToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GetValueToDoItemSettingsCore(id, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<ValueToDoItemSettings>> GetValueToDoItemSettingsCore(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<ValueToDoItemSettings>(item).ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<PeriodicityToDoItemSettings>> GetPeriodicityToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GetPeriodicityToDoItemSettingsCore(id, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<PeriodicityToDoItemSettings>> GetPeriodicityToDoItemSettingsCore(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<PeriodicityToDoItemSettings>(item).ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<WeeklyPeriodicity>> GetWeeklyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GetWeeklyPeriodicityCore(id, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<WeeklyPeriodicity>> GetWeeklyPeriodicityCore(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<WeeklyPeriodicity>(item).ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<MonthlyPeriodicity>> GetMonthlyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GetMonthlyPeriodicityCore(id, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<MonthlyPeriodicity>> GetMonthlyPeriodicityCore(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<MonthlyPeriodicity>(item).ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<AnnuallyPeriodicity>> GetAnnuallyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GetAnnuallyPeriodicityCore(id, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result<AnnuallyPeriodicity>> GetAnnuallyPeriodicityCore(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<AnnuallyPeriodicity>(item).ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<PeriodicityOffsetToDoItemSettings>>
        GetPeriodicityOffsetToDoItemSettingsAsync(
            Guid id,
            CancellationToken cancellationToken
        )
    {
        return GetPeriodicityOffsetToDoItemSettingsCore(id, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<PeriodicityOffsetToDoItemSettings>> GetPeriodicityOffsetToDoItemSettingsCore(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.Set<ToDoItemEntity>().FindAsync(id);
        item = item.ThrowIfNull();

        return mapper.Map<PeriodicityOffsetToDoItemSettings>(item).ToResult();
    }

    public async IAsyncEnumerable<ReadOnlyMemory<ToDoItem>> GetToDoItemsAsync(
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
                if (items.Count > 0)
                {
                    yield return items.ToArray();

                    items.Clear();
                }

            items.Add((await GetToDoItemAsync(ids[i], cancellationToken)).Value);
        }

        if (items.Count > 0)
        {
            yield return items.ToArray();

            items.Clear();
        }
    }

    private async ValueTask ToDoItemToStringAsync(
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

            if (!options.Statuses.Span.ToArray().Contains(parameters.Status)) continue;

            builder.Duplicate(" ", level);
            builder.Append(item.Name);
            builder.AppendLine();

            await ToDoItemToStringAsync(
                context,
                new ToDoItemToStringOptions(options.Statuses.ToArray(), item.Id),
                (ushort)(level + 1),
                builder,
                offset,
                cancellationToken
            );
        }
    }

    private async ValueTask<ToDoSelectorItem[]> GetToDoSelectorItemsAsync(
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
            result[i] = new ToDoSelectorItem(item.Id, item.Name, children);
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

    private async ValueTask NormalizeOrderIndexAsync(
        SpravyDbToDoDbContext context,
        Guid? parentId,
        CancellationToken cancellationToken
    )
    {
        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == parentId)
            .ToArrayAsync(cancellationToken);

        var ordered = items.OrderBy(x => x.OrderIndex).ToArray();

        for (var index = 0u; index < ordered.LongLength; index++) ordered[index].OrderIndex = index;
    }

    private async ValueTask GetParentsAsync(
        SpravyDbToDoDbContext context,
        Guid id,
        List<ToDoShortItem> parents,
        CancellationToken cancellationToken
    )
    {
        var parent = await context.Set<ToDoItemEntity>()
            .Include(x => x.Parent)
            .SingleAsync(x => x.Id == id, cancellationToken);

        if (parent.Parent is null) return;

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
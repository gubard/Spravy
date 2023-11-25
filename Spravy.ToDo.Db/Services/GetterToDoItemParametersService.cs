using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Db.Services;

public class GetterToDoItemParametersService
{
    public async Task<ToDoItemParameters> GetToDoItemParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        var parameters = new ToDoItemParameters();

        parameters = await (entity.Type switch
        {
            ToDoItemType.Value => GetValueParametersAsync(context, entity, offset, parameters),
            ToDoItemType.Group => GetGroupParametersAsync(context, entity, offset, parameters),
            ToDoItemType.Planned => GetPlannedParametersAsync(context, entity, offset, parameters),
            ToDoItemType.Periodicity => GetPeriodicityParametersAsync(context, entity, offset, parameters),
            ToDoItemType.PeriodicityOffset => GetPeriodicityOffsetParametersAsync(context, entity, offset, parameters),
            ToDoItemType.Circle => GetCircleParametersAsync(context, entity, offset, parameters),
            ToDoItemType.Step => GetStepParametersAsync(context, entity, offset, parameters),
            _ => throw new ArgumentOutOfRangeException()
        });

        return CheckActiveItem(parameters, entity);
    }

    private ToDoItemParameters CheckActiveItem(ToDoItemParameters parameters, ToDoItemEntity entity)
    {
        return parameters.ActiveItem.Value.HasValue && parameters.ActiveItem.Value.Value.Id == entity.ParentId
            ? parameters.Set(null)
            : parameters;
    }

    private async Task<ToDoItemParameters> GetPlannedParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset,
        ToDoItemParameters parameters
    )
    {
        if (parameters.IsSuccess)
        {
            return parameters;
        }

        if (entity.IsCompleted)
        {
            return parameters.WithIfNeed(ToDoItemIsCan.CanIncomplete)
                .WithIfNeed(null)
                .WithIfNeed(ToDoItemStatus.Completed, null);
        }

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var ai = ToActiveToDoItem(entity);

        if (items.Length == 0)
        {
            if (entity.DueDate == DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
            {
                return parameters.WithIfNeed(ToActiveToDoItem(entity))
                    .WithIfNeed(ToDoItemStatus.ReadyForComplete, ai)
                    .WithIfNeed(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
            }
        }

        if (entity.DueDate < DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return parameters.WithIfNeed(ToActiveToDoItem(entity))
                .WithIfNeed(ToDoItemStatus.Miss, ai)
                .WithIfNeed(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
        }

        if (entity.DueDate > DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return parameters.WithIfNeed(null)
                .WithIfNeed(ToDoItemStatus.Planned, null)
                .WithIfNeed(ToDoItemIsCan.None);
        }

        foreach (var item in items)
        {
            parameters = await GetToDoItemParametersAsync(context, item, offset, parameters);
        }

        switch (entity.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
            {
                if (parameters.ActiveItem is { IsSuccess: true, Value: not null })
                {
                    return parameters.Set(ToDoItemIsCan.None);
                }

                break;
            }
            case ToDoItemChildrenType.IgnoreCompletion:
                return parameters.Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
            default: throw new ArgumentOutOfRangeException();
        }

        return parameters;
    }

    private async Task<ToDoItemParameters> GetPeriodicityOffsetParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset,
        ToDoItemParameters parameters
    )
    {
        if (parameters.IsSuccess)
        {
            return parameters;
        }

        if (entity.IsCompleted)
        {
            return parameters.WithIfNeed(ToDoItemIsCan.CanIncomplete)
                .WithIfNeed(null)
                .WithIfNeed(ToDoItemStatus.Completed, null);
        }

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var ai = ToActiveToDoItem(entity);

        if (items.Length == 0)
        {
            if (entity.DueDate == DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
            {
                return parameters.WithIfNeed(ai)
                    .WithIfNeed(ToDoItemStatus.ReadyForComplete, ai)
                    .WithIfNeed(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
            }
        }

        if (entity.DueDate < DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return parameters.WithIfNeed(ai)
                .WithIfNeed(ToDoItemStatus.Miss, ai)
                .WithIfNeed(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
        }

        if (entity.DueDate > DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return parameters.WithIfNeed(null)
                .WithIfNeed(ToDoItemStatus.Planned, null)
                .WithIfNeed(ToDoItemIsCan.None);
        }


        foreach (var item in items)
        {
            parameters = await GetToDoItemParametersAsync(context, item, offset, parameters);
        }

        switch (entity.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
            {
                if (parameters.ActiveItem is { IsSuccess: true, Value: not null })
                {
                    return parameters.Set(ToDoItemIsCan.None);
                }

                break;
            }
            case ToDoItemChildrenType.IgnoreCompletion:
                return parameters.Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
            default: throw new ArgumentOutOfRangeException();
        }
        
        if (entity.DueDate == DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return parameters.WithIfNeed(ai)
                .WithIfNeed(ToDoItemStatus.ReadyForComplete, ai)
                .Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
        }

        return parameters;
    }

    private async Task<ToDoItemParameters> GetPeriodicityParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset,
        ToDoItemParameters parameters
    )
    {
        if (parameters.IsSuccess)
        {
            return parameters;
        }

        if (entity.IsCompleted)
        {
            return parameters.WithIfNeed(ToDoItemIsCan.CanIncomplete)
                .WithIfNeed(null)
                .WithIfNeed(ToDoItemStatus.Completed, null);
        }

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        var ai = ToActiveToDoItem(entity);

        if (items.Length == 0)
        {
            if (entity.DueDate == DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
            {
                return parameters.WithIfNeed(ai)
                    .WithIfNeed(ToDoItemStatus.ReadyForComplete, ai)
                    .WithIfNeed(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
            }
        }

        if (entity.DueDate < DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return parameters.WithIfNeed(ai)
                .WithIfNeed(ToDoItemStatus.Miss, ai)
                .WithIfNeed(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
        }

        if (entity.DueDate > DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return parameters.WithIfNeed(null)
                .WithIfNeed(ToDoItemStatus.Planned, null)
                .WithIfNeed(ToDoItemIsCan.None);
        }

        foreach (var item in items)
        {
            parameters = await GetToDoItemParametersAsync(context, item, offset, parameters);
        }

        switch (entity.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
            {
                if (parameters.ActiveItem is { IsSuccess: true, Value: not null })
                {
                    return parameters.Set(ToDoItemIsCan.None);
                }

                break;
            }
            case ToDoItemChildrenType.IgnoreCompletion:
                return parameters.Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
            default: throw new ArgumentOutOfRangeException();
        }

        return parameters;
    }

    private async Task<ToDoItemParameters> GetGroupParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset,
        ToDoItemParameters parameters
    )
    {
        if (parameters.IsSuccess)
        {
            return parameters;
        }

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        if (items.Length == 0)
        {
            return parameters.WithIfNeed(null)
                .WithIfNeed(ToDoItemStatus.Completed, null)
                .WithIfNeed(ToDoItemIsCan.None);
        }

        foreach (var item in items)
        {
            parameters = await GetToDoItemParametersAsync(context, item, offset, parameters);
        }

        return parameters;
    }

    private async Task<ToDoItemParameters> GetCircleParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset,
        ToDoItemParameters parameters
    )
    {
        if (parameters.IsSuccess)
        {
            return parameters;
        }

        if (entity.IsCompleted)
        {
            return parameters.WithIfNeed(ToDoItemIsCan.CanIncomplete)
                .WithIfNeed(null)
                .WithIfNeed(ToDoItemStatus.Completed, null);
        }

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        if (items.Length == 0)
        {
            var ai = ToActiveToDoItem(entity);

            return parameters.WithIfNeed(ToActiveToDoItem(entity))
                .WithIfNeed(ToDoItemStatus.ReadyForComplete, ai)
                .WithIfNeed(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
        }

        foreach (var item in items)
        {
            parameters = await GetToDoItemParametersAsync(context, item, offset, parameters);
        }

        switch (entity.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
            {
                if (parameters.ActiveItem is { IsSuccess: true, Value: not null })
                {
                    return parameters.Set(ToDoItemIsCan.None);
                }

                break;
            }
            case ToDoItemChildrenType.IgnoreCompletion:
                return parameters.Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
            default: throw new ArgumentOutOfRangeException();
        }
        
        if (!parameters.ActiveItem.Value.HasValue)
        {
            return parameters.Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip).Set(ToDoItemStatus.ReadyForComplete);
        }

        return parameters;
    }

    private async Task<ToDoItemParameters> GetStepParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset,
        ToDoItemParameters parameters
    )
    {
        if (parameters.IsSuccess)
        {
            return parameters;
        }

        if (entity.IsCompleted)
        {
            return parameters.WithIfNeed(ToDoItemIsCan.CanIncomplete)
                .WithIfNeed(null)
                .WithIfNeed(ToDoItemStatus.Completed, null);
        }

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        if (items.Length == 0)
        {
            var ai = ToActiveToDoItem(entity);

            return parameters.WithIfNeed(ai)
                .WithIfNeed(ToDoItemStatus.ReadyForComplete, ai)
                .WithIfNeed(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
        }

        foreach (var item in items)
        {
            parameters = await GetToDoItemParametersAsync(context, item, offset, parameters);
        }

        switch (entity.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
            {
                if (parameters.ActiveItem is { IsSuccess: true, Value: not null })
                {
                    return parameters.Set(ToDoItemIsCan.None);
                }

                break;
            }
            case ToDoItemChildrenType.IgnoreCompletion:
                return parameters.Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
            default: throw new ArgumentOutOfRangeException();
        }

        if (!parameters.ActiveItem.Value.HasValue)
        {
            return parameters.Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip).Set(ToDoItemStatus.ReadyForComplete);
        }

        return parameters;
    }

    private async Task<ToDoItemParameters> GetValueParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset,
        ToDoItemParameters parameters
    )
    {
        if (parameters.IsSuccess)
        {
            return parameters;
        }

        if (entity.IsCompleted)
        {
            return parameters.WithIfNeed(ToDoItemIsCan.CanIncomplete)
                .WithIfNeed(null)
                .WithIfNeed(ToDoItemStatus.Completed, null);
        }

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        if (items.Length == 0)
        {
            var ai = ToActiveToDoItem(entity);

            return parameters.WithIfNeed(ai)
                .WithIfNeed(ToDoItemStatus.ReadyForComplete, ai)
                .WithIfNeed(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
        }

        foreach (var item in items)
        {
            parameters = await GetToDoItemParametersAsync(context, item, offset, parameters);
        }

        switch (entity.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
            {
                if (parameters.ActiveItem is { IsSuccess: true, Value: not null })
                {
                    return parameters.Set(ToDoItemIsCan.None);
                }

                break;
            }
            case ToDoItemChildrenType.IgnoreCompletion:
                return parameters.Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
            default: throw new ArgumentOutOfRangeException();
        }
        
        if (!parameters.ActiveItem.Value.HasValue)
        {
            return parameters.Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip).Set(ToDoItemStatus.ReadyForComplete);
        }

        return parameters;
    }

    private Task<ToDoItemParameters> GetToDoItemParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset,
        ToDoItemParameters parameters
    )
    {
        return entity.Type switch
        {
            ToDoItemType.Value => GetValueParametersAsync(context, entity, offset, parameters),
            ToDoItemType.Group => GetGroupParametersAsync(context, entity, offset, parameters),
            ToDoItemType.Planned => GetPlannedParametersAsync(context, entity, offset, parameters),
            ToDoItemType.Periodicity => GetPeriodicityParametersAsync(context, entity, offset, parameters),
            ToDoItemType.PeriodicityOffset => GetPeriodicityOffsetParametersAsync(context, entity, offset, parameters),
            ToDoItemType.Circle => GetCircleParametersAsync(context, entity, offset, parameters),
            ToDoItemType.Step => GetStepParametersAsync(context, entity, offset, parameters),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private ActiveToDoItem? ToActiveToDoItem(ToDoItemEntity entity)
    {
        return entity.ParentId is null ? null : new ActiveToDoItem(entity.ParentId.Value, entity.Name);
    }
}
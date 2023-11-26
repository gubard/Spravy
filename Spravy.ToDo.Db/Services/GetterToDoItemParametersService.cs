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
        TimeSpan offset,
        CancellationToken cancellationToken
    )
    {
        var parameters = new ToDoItemParameters();
        parameters = await GetToDoItemParametersAsync(context, entity, offset, parameters, cancellationToken);

        return CheckActiveItem(parameters, entity);
    }

    public async Task<ToDoItemParameters> GetToDoItemParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset,
        ToDoItemParameters parameters,
        CancellationToken cancellationToken
    )
    {
        if (entity.IsCompleted && IsCompletable(entity))
        {
            return parameters.Set(ToDoItemIsCan.CanIncomplete)
                .Set(null)
                .Set(ToDoItemStatus.Completed);
        }

        if (IsDueable(entity))
        {
            if (entity.DueDate < DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
            {
                return parameters.Set(ToActiveToDoItem(entity))
                    .Set(ToDoItemStatus.Miss)
                    .Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip);
            }

            if (entity.DueDate > DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
            {
                return parameters.Set(null)
                    .Set(ToDoItemStatus.Planned)
                    .Set(ToDoItemIsCan.None);
            }
        }

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        ActiveToDoItem? firstReadyForComplete = null;
        var hasPlanned = false;

        foreach (var item in items)
        {
            parameters = await GetToDoItemParametersAsync(context, item, offset, parameters, cancellationToken);

            switch (parameters.Status)
            {
                case ToDoItemStatus.Miss:
                    if (IsGroup(entity))
                    {
                        return parameters.Set(ToDoItemStatus.Miss).Set(ToDoItemIsCan.None);
                    }

                    return parameters.Set(ToDoItemStatus.Miss);
                case ToDoItemStatus.ReadyForComplete:
                    firstReadyForComplete = parameters.ActiveItem ?? ToActiveToDoItem(item);
                    break;
                case ToDoItemStatus.Planned:
                    hasPlanned = true;
                    break;
                case ToDoItemStatus.Completed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (IsGroup(entity))
        {
            if (firstReadyForComplete is null)
            {
                if (hasPlanned)
                {
                    return parameters.Set(ToDoItemStatus.Planned).Set(ToDoItemIsCan.None).Set(null);
                }
            }
            else
            {
                return parameters.Set(ToDoItemStatus.ReadyForComplete)
                    .Set(ToDoItemIsCan.None)
                    .Set(firstReadyForComplete);
            }
        }

        if (firstReadyForComplete is null)
        {
            return parameters.Set(ToDoItemStatus.ReadyForComplete)
                .Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip)
                .Set(null);
        }

        switch (entity.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
                return parameters.Set(ToDoItemStatus.ReadyForComplete)
                    .Set(ToDoItemIsCan.None)
                    .Set(firstReadyForComplete);
            case ToDoItemChildrenType.IgnoreCompletion:
                return parameters.Set(ToDoItemStatus.ReadyForComplete)
                    .Set(ToDoItemIsCan.CanFail | ToDoItemIsCan.CanComplete | ToDoItemIsCan.CanSkip)
                    .Set(firstReadyForComplete);
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private bool IsDueable(ToDoItemEntity entity)
    {
        return entity.Type switch
        {
            ToDoItemType.Value => false,
            ToDoItemType.Group => false,
            ToDoItemType.Planned => true,
            ToDoItemType.Periodicity => true,
            ToDoItemType.PeriodicityOffset => true,
            ToDoItemType.Circle => false,
            ToDoItemType.Step => false,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private bool IsCompletable(ToDoItemEntity entity)
    {
        return entity.Type switch
        {
            ToDoItemType.Value => true,
            ToDoItemType.Group => false,
            ToDoItemType.Planned => true,
            ToDoItemType.Periodicity => false,
            ToDoItemType.PeriodicityOffset => false,
            ToDoItemType.Circle => true,
            ToDoItemType.Step => true,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private bool IsGroup(ToDoItemEntity entity)
    {
        return entity.Type switch
        {
            ToDoItemType.Value => false,
            ToDoItemType.Group => true,
            ToDoItemType.Planned => false,
            ToDoItemType.Periodicity => false,
            ToDoItemType.PeriodicityOffset => false,
            ToDoItemType.Circle => false,
            ToDoItemType.Step => false,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private ToDoItemParameters CheckActiveItem(ToDoItemParameters parameters, ToDoItemEntity entity)
    {
        return parameters.ActiveItem.HasValue && parameters.ActiveItem.Value.Id == entity.ParentId
            ? parameters.Set(null)
            : parameters;
    }

    private ActiveToDoItem? ToActiveToDoItem(ToDoItemEntity entity)
    {
        return entity.ParentId is null ? null : new ActiveToDoItem(entity.ParentId.Value, entity.Name);
    }
}
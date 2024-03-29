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

    private Task<ToDoItemParameters> GetToDoItemParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset,
        ToDoItemParameters parameters,
        CancellationToken cancellationToken
    )
    {
        if (IsDueable(entity))
        {
            return GetToDoItemParametersAsync(
                context,
                entity,
                entity.DueDate,
                offset,
                parameters,
                false,
                cancellationToken
            );
        }

        return GetToDoItemParametersAsync(
            context,
            entity,
            DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly(),
            offset,
            parameters,
            false,
            cancellationToken
        );
    }

    private async Task<ToDoItemParameters> GetToDoItemParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        DateOnly dueDate,
        TimeSpan offset,
        ToDoItemParameters parameters,
        bool useDueDate,
        CancellationToken cancellationToken
    )
    {
        if (entity.IsCompleted && IsCompletable(entity))
        {
            return parameters.With(ToDoItemIsCan.CanIncomplete)
                .With(null)
                .With(ToDoItemStatus.Completed);
        }

        var isMiss = false;

        if (IsDueable(entity))
        {
            if (useDueDate)
            {
                if (entity.DueDate < dueDate && entity.IsRequiredCompleteInDueDate)
                {
                    isMiss = true;
                }

                if (entity.DueDate > dueDate)
                {
                    return parameters.With(null)
                        .With(ToDoItemStatus.Planned)
                        .With(ToDoItemIsCan.None);
                }
            }
            else
            {
                if (entity.DueDate < DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly()
                    && entity.IsRequiredCompleteInDueDate)
                {
                    isMiss = true;
                }

                if (entity.DueDate > DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
                {
                    return parameters.With(null)
                        .With(ToDoItemStatus.Planned)
                        .With(ToDoItemIsCan.None);
                }
            }
        }

        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(cancellationToken);

        ActiveToDoItem? firstReadyForComplete = null;
        ActiveToDoItem? firstMiss = null;
        var hasPlanned = false;

        foreach (var item in items)
        {
            if (firstMiss.HasValue)
            {
                break;
            }

            parameters = await GetToDoItemParametersAsync(
                context,
                item,
                dueDate,
                offset,
                parameters,
                true,
                cancellationToken
            );

            switch (parameters.Status)
            {
                case ToDoItemStatus.Miss:
                    firstMiss ??= parameters.ActiveItem ?? ToActiveToDoItem(item);
                    break;
                case ToDoItemStatus.ReadyForComplete:
                    firstReadyForComplete ??= parameters.ActiveItem ?? ToActiveToDoItem(item);

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

        var firstActive = firstMiss ?? firstReadyForComplete;

        if (IsGroup(entity))
        {
            if (isMiss)
            {
                return parameters.With(ToDoItemStatus.Miss)
                    .With(ToDoItemIsCan.None)
                    .With(firstActive ?? ToActiveToDoItem(entity));
            }

            if (firstMiss is not null)
            {
                return parameters.With(ToDoItemStatus.Miss).With(ToDoItemIsCan.None).With(firstMiss);
            }

            if (firstReadyForComplete is null)
            {
                if (hasPlanned)
                {
                    return parameters.With(ToDoItemStatus.Planned).With(ToDoItemIsCan.None).With(null);
                }

                return parameters.With(ToDoItemStatus.Completed).With(ToDoItemIsCan.None).With(null);
            }

            return parameters.With(ToDoItemStatus.ReadyForComplete)
                .With(ToDoItemIsCan.None)
                .With(firstReadyForComplete);
        }

        if (isMiss)
        {
            switch (entity.ChildrenType)
            {
                case ToDoItemChildrenType.RequireCompletion:
                    if (firstActive.HasValue)
                    {
                        return parameters.With(ToDoItemStatus.Miss)
                            .With(ToDoItemIsCan.None)
                            .With(firstActive);
                    }

                    return parameters.With(ToDoItemStatus.Miss)
                        .With(ToDoItemIsCan.CanComplete)
                        .With(ToActiveToDoItem(entity));
                case ToDoItemChildrenType.IgnoreCompletion:
                    return parameters.With(ToDoItemStatus.Miss)
                        .With(ToDoItemIsCan.CanComplete)
                        .With(firstActive ?? ToActiveToDoItem(entity));
                default: throw new ArgumentOutOfRangeException();
            }
        }

        if (firstMiss is not null)
        {
            switch (entity.ChildrenType)
            {
                case ToDoItemChildrenType.RequireCompletion:
                    return parameters.With(ToDoItemStatus.Miss)
                        .With(ToDoItemIsCan.None)
                        .With(firstMiss);
                case ToDoItemChildrenType.IgnoreCompletion:
                    return parameters.With(ToDoItemStatus.Miss)
                        .With(ToDoItemIsCan.CanComplete)
                        .With(firstMiss);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        if (firstReadyForComplete is not null)
        {
            switch (entity.ChildrenType)
            {
                case ToDoItemChildrenType.RequireCompletion:
                    return parameters.With(ToDoItemStatus.ReadyForComplete)
                        .With(ToDoItemIsCan.None)
                        .With(firstReadyForComplete);
                case ToDoItemChildrenType.IgnoreCompletion:
                    return parameters.With(ToDoItemStatus.ReadyForComplete)
                        .With(ToDoItemIsCan.CanComplete)
                        .With(firstReadyForComplete);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        return parameters.With(ToDoItemStatus.ReadyForComplete)
            .With(ToDoItemIsCan.CanComplete)
            .With(null);
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
            ? parameters.With(null)
            : parameters;
    }

    private ActiveToDoItem? ToActiveToDoItem(ToDoItemEntity entity)
    {
        return entity.ParentId is null ? null : new ActiveToDoItem(entity.ParentId.Value, entity.Name);
    }
}
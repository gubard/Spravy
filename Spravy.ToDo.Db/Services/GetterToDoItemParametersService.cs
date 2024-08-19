using System.Runtime.CompilerServices;
using Spravy.Core.Mappers;
using Spravy.Db.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Errors;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Db.Services;

public class GetterToDoItemParametersService
{
    public ConfiguredValueTaskAwaitable<Result<ToDoItemParameters>> GetToDoItemParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset,
        CancellationToken ct
    )
    {
        return GetToDoItemParametersAsync(context, entity, offset, new(), ct);
    }

    private ConfiguredValueTaskAwaitable<Result<ToDoItemParameters>> GetToDoItemParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset,
        ToDoItemParameters parameters,
        CancellationToken ct
    )
    {
        return IsDueableAsync(context, entity, ct)
            .IfSuccessAsync(
                isDueable =>
                {
                    if (isDueable)
                    {
                        return GetToDoItemParametersAsync(
                                context,
                                entity,
                                entity.DueDate,
                                offset,
                                parameters,
                                false,
                                new(),
                                ct
                            )
                            .ConfigureAwait(false);
                    }

                    return GetToDoItemParametersAsync(
                            context,
                            entity,
                            DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly(),
                            offset,
                            parameters,
                            false,
                            new(),
                            ct
                        )
                        .ConfigureAwait(false);
                },
                ct
            );
    }

    private async ValueTask<Result<ToDoItemParameters>> GetToDoItemParametersAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        DateOnly dueDate,
        TimeSpan offset,
        ToDoItemParameters parameters,
        bool useDueDate,
        List<Guid> ignoreIds,
        CancellationToken ct
    )
    {
        if (entity.Type == ToDoItemType.Reference)
        {
            if (entity.ReferenceId.HasValue)
            {
                ignoreIds.Add(entity.Id);

                return await context
                    .GetEntityAsync<ToDoItemEntity>(entity.ReferenceId.Value)
                    .IfSuccessAsync(
                        item =>
                            GetToDoItemParametersAsync(
                                    context,
                                    item,
                                    dueDate,
                                    offset,
                                    parameters,
                                    useDueDate,
                                    ignoreIds,
                                    ct
                                )
                                .ConfigureAwait(false),
                        ct
                    );
            }

            return parameters
                .With(ToDoItemIsCan.None)
                .With(new OptionStruct<ActiveToDoItem>())
                .With(ToDoItemStatus.Miss)
                .ToResult();
        }

        var isCompletable = IsCompletable(entity);

        if (!isCompletable.TryGetValue(out var i))
        {
            return new(isCompletable.Errors);
        }

        if (entity.IsCompleted && i)
        {
            return parameters
                .With(ToDoItemIsCan.CanIncomplete)
                .With(new OptionStruct<ActiveToDoItem>())
                .With(ToDoItemStatus.Completed)
                .ToResult();
        }

        var isMiss = false;
        var isDueable = await IsDueableAsync(context, entity, ct);

        if (!isDueable.TryGetValue(out var rv))
        {
            return new(isDueable.Errors);
        }

        if (rv)
        {
            if (useDueDate)
            {
                if (entity.DueDate < dueDate && entity.IsRequiredCompleteInDueDate)
                {
                    isMiss = true;
                }

                if (entity.DueDate > dueDate)
                {
                    return parameters
                        .With(new OptionStruct<ActiveToDoItem>())
                        .With(ToDoItemStatus.Planned)
                        .With(ToDoItemIsCan.None)
                        .ToResult();
                }
            }
            else
            {
                if (
                    entity.DueDate < DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly()
                    && entity.IsRequiredCompleteInDueDate
                )
                {
                    isMiss = true;
                }

                if (entity.DueDate > DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
                {
                    return parameters
                        .With(new OptionStruct<ActiveToDoItem>())
                        .With(ToDoItemStatus.Planned)
                        .With(ToDoItemIsCan.None)
                        .ToResult();
                }
            }
        }

        var items = await context
            .Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id && !ignoreIds.Contains(x.Id))
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(ct);

        var firstReadyForComplete = new OptionStruct<ActiveToDoItem>();
        var firstMiss = new OptionStruct<ActiveToDoItem>();
        var hasPlanned = false;

        foreach (var item in items)
        {
            if (firstMiss.IsHasValue)
            {
                break;
            }

            var result = await GetToDoItemParametersAsync(
                context,
                item,
                dueDate,
                offset,
                parameters,
                true,
                ignoreIds,
                ct
            );

            if (!result.TryGetValue(out var r))
            {
                return result;
            }

            parameters = r;

            switch (parameters.Status)
            {
                case ToDoItemStatus.Miss:
                {
                    if (firstMiss.IsHasValue)
                    {
                        break;
                    }

                    firstMiss = parameters.ActiveItem.TryGetValue(out var value)
                        ? value.ToOption()
                        : ToActiveToDoItem(item);

                    break;
                }
                case ToDoItemStatus.ReadyForComplete:
                {
                    if (firstReadyForComplete.IsHasValue)
                    {
                        break;
                    }

                    firstReadyForComplete = parameters.ActiveItem.TryGetValue(out var value)
                        ? value.ToOption()
                        : ToActiveToDoItem(item);

                    break;
                }
                case ToDoItemStatus.Planned:
                    hasPlanned = true;

                    break;
                case ToDoItemStatus.Completed:
                    break;
                default:
                    return new(new ToDoItemStatusOutOfRangeError(parameters.Status));
            }
        }

        var firstActive = firstMiss.IsHasValue ? firstMiss : firstReadyForComplete;
        var isGroup = IsGroup(entity);

        if (!isGroup.TryGetValue(out var g))
        {
            return new(isGroup.Errors);
        }

        if (g)
        {
            if (isMiss)
            {
                return parameters
                    .With(ToDoItemStatus.Miss)
                    .With(ToDoItemIsCan.None)
                    .With(firstActive.IsHasValue ? firstActive : ToActiveToDoItem(entity))
                    .ToResult();
            }

            if (firstMiss.IsHasValue)
            {
                return parameters
                    .With(ToDoItemStatus.Miss)
                    .With(ToDoItemIsCan.None)
                    .With(firstMiss)
                    .ToResult();
            }

            if (!firstReadyForComplete.IsHasValue)
            {
                if (hasPlanned)
                {
                    return parameters
                        .With(ToDoItemStatus.Planned)
                        .With(ToDoItemIsCan.None)
                        .With(new OptionStruct<ActiveToDoItem>())
                        .ToResult();
                }

                return parameters
                    .With(ToDoItemStatus.Completed)
                    .With(ToDoItemIsCan.None)
                    .With(new OptionStruct<ActiveToDoItem>())
                    .ToResult();
            }

            return parameters
                .With(ToDoItemStatus.ReadyForComplete)
                .With(ToDoItemIsCan.None)
                .With(firstReadyForComplete)
                .ToResult();
        }

        if (isMiss)
        {
            switch (entity.ChildrenType)
            {
                case ToDoItemChildrenType.RequireCompletion:
                    if (firstActive.IsHasValue)
                    {
                        return parameters
                            .With(ToDoItemStatus.Miss)
                            .With(ToDoItemIsCan.None)
                            .With(firstActive)
                            .ToResult();
                    }

                    return parameters
                        .With(ToDoItemStatus.Miss)
                        .With(ToDoItemIsCan.CanComplete)
                        .With(ToActiveToDoItem(entity))
                        .ToResult();
                case ToDoItemChildrenType.IgnoreCompletion:
                    return parameters
                        .With(ToDoItemStatus.Miss)
                        .With(ToDoItemIsCan.CanComplete)
                        .With(firstActive.IsHasValue ? firstActive : ToActiveToDoItem(entity))
                        .ToResult();
                default:
                    return new(new ToDoItemChildrenTypeOutOfRangeError(entity.ChildrenType));
            }
        }

        if (firstMiss.IsHasValue)
        {
            switch (entity.ChildrenType)
            {
                case ToDoItemChildrenType.RequireCompletion:
                    return parameters
                        .With(ToDoItemStatus.Miss)
                        .With(ToDoItemIsCan.None)
                        .With(firstMiss)
                        .ToResult();
                case ToDoItemChildrenType.IgnoreCompletion:
                    return parameters
                        .With(ToDoItemStatus.Miss)
                        .With(ToDoItemIsCan.CanComplete)
                        .With(firstMiss)
                        .ToResult();
                default:
                    return new(new ToDoItemChildrenTypeOutOfRangeError(entity.ChildrenType));
            }
        }

        if (firstReadyForComplete.IsHasValue)
        {
            switch (entity.ChildrenType)
            {
                case ToDoItemChildrenType.RequireCompletion:
                    return parameters
                        .With(ToDoItemStatus.ReadyForComplete)
                        .With(ToDoItemIsCan.None)
                        .With(firstReadyForComplete)
                        .ToResult();
                case ToDoItemChildrenType.IgnoreCompletion:
                    return parameters
                        .With(ToDoItemStatus.ReadyForComplete)
                        .With(ToDoItemIsCan.CanComplete)
                        .With(firstReadyForComplete)
                        .ToResult();
                default:
                    return new(new ToDoItemChildrenTypeOutOfRangeError(entity.ChildrenType));
            }
        }

        return parameters
            .With(ToDoItemStatus.ReadyForComplete)
            .With(ToDoItemIsCan.CanComplete)
            .With(new OptionStruct<ActiveToDoItem>())
            .ToResult();
    }

    private ConfiguredValueTaskAwaitable<Result<bool>> IsDueableAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        CancellationToken ct
    )
    {
        return entity.Type switch
        {
            ToDoItemType.Value => false.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.Group => false.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.Planned => true.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.Periodicity => true.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.PeriodicityOffset
                => true.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.Circle => false.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.Step => false.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.Reference
                => entity.ReferenceId.HasValue
                    ? context
                        .GetEntityAsync<ToDoItemEntity>(entity.ReferenceId.Value)
                        .IfSuccessAsync(item => IsDueableAsync(context, item, ct), ct)
                    : false.ToResult().ToValueTaskResult().ConfigureAwait(false),
            _
                => new Result<bool>(new ToDoItemTypeOutOfRangeError(entity.Type))
                    .ToValueTaskResult()
                    .ConfigureAwait(false),
        };
    }

    private ConfiguredValueTaskAwaitable<Result<DateOnly>> GetDueDateAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        CancellationToken ct
    )
    {
        return entity.Type switch
        {
            ToDoItemType.Value
                => entity.DueDate.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.Group
                => entity.DueDate.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.Planned
                => entity.DueDate.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.Periodicity
                => entity.DueDate.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.PeriodicityOffset
                => entity.DueDate.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.Circle
                => entity.DueDate.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.Step
                => entity.DueDate.ToResult().ToValueTaskResult().ConfigureAwait(false),
            ToDoItemType.Reference
                => entity.ReferenceId.HasValue
                    ? context
                        .GetEntityAsync<ToDoItemEntity>(entity.ReferenceId.Value)
                        .IfSuccessAsync(item => GetDueDateAsync(context, item, ct), ct)
                    : DateOnly.MinValue.ToResult().ToValueTaskResult().ConfigureAwait(false),
            _
                => new Result<DateOnly>(new ToDoItemTypeOutOfRangeError(entity.Type))
                    .ToValueTaskResult()
                    .ConfigureAwait(false),
        };
    }

    private Result<bool> IsCompletable(ToDoItemEntity entity)
    {
        return entity.Type switch
        {
            ToDoItemType.Value => true.ToResult(),
            ToDoItemType.Group => false.ToResult(),
            ToDoItemType.Planned => true.ToResult(),
            ToDoItemType.Periodicity => false.ToResult(),
            ToDoItemType.PeriodicityOffset => false.ToResult(),
            ToDoItemType.Circle => true.ToResult(),
            ToDoItemType.Step => true.ToResult(),
            ToDoItemType.Reference => false.ToResult(),
            _ => new(new ToDoItemTypeOutOfRangeError(entity.Type)),
        };
    }

    private Result<bool> IsGroup(ToDoItemEntity entity)
    {
        return entity.Type switch
        {
            ToDoItemType.Value => false.ToResult(),
            ToDoItemType.Group => true.ToResult(),
            ToDoItemType.Planned => false.ToResult(),
            ToDoItemType.Periodicity => false.ToResult(),
            ToDoItemType.PeriodicityOffset => false.ToResult(),
            ToDoItemType.Circle => false.ToResult(),
            ToDoItemType.Step => false.ToResult(),
            ToDoItemType.Reference => false.ToResult(),
            _ => new(new ToDoItemTypeOutOfRangeError(entity.Type)),
        };
    }

    private OptionStruct<ActiveToDoItem> ToActiveToDoItem(ToDoItemEntity entity)
    {
        return entity.ParentId is null
            ? new()
            : new ActiveToDoItem(entity.Id, entity.Name, entity.ParentId.ToOption()).ToOption();
    }
}

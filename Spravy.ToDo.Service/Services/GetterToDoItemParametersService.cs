using System.Collections.Frozen;

namespace Spravy.ToDo.Service.Services;

public class GetterToDoItemParametersService
{
    private readonly ILogger<GetterToDoItemParametersService> logger;

    public GetterToDoItemParametersService(ILogger<GetterToDoItemParametersService> logger)
    {
        this.logger = logger;
    }

    public Result<ToDoItemParameters> GetToDoItemParameters(
        FrozenDictionary<Guid, ToDoItemEntity> allItems,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        return GetToDoItemParameters(allItems, entity, offset, new())
           .IfSuccess(
                parameters =>
                {
                    if (!parameters.ActiveItem.TryGetValue(out var active))
                    {
                        return parameters.ToResult();
                    }

                    if (active.Id == entity.Id)
                    {
                        return parameters.With(OptionStruct<ToDoShortItem>.Default).ToResult();
                    }

                    return parameters.ToResult();
                }
            )
           .IfSuccess(
                parameters =>
                {
                    var today = DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly();

                    if (parameters.Status == ToDoItemStatus.Planned
                     && entity.RemindDaysBefore != 0
                     && today >= entity.DueDate.AddDays((int)-entity.RemindDaysBefore))
                    {
                        return parameters.With(ToDoItemStatus.ComingSoon).ToResult();
                    }

                    return parameters.ToResult();
                }
            );
    }

    private Result<ToDoItemParameters> GetToDoItemParameters(
        FrozenDictionary<Guid, ToDoItemEntity> allItems,
        ToDoItemEntity entity,
        TimeSpan offset,
        ToDoItemParameters parameters
    )
    {
        return IsDueable(allItems, entity)
           .IfSuccess(
                isDueable =>
                {
                    if (isDueable)
                    {
                        return GetToDoItemParameters(
                            allItems,
                            entity,
                            entity.DueDate,
                            offset,
                            parameters,
                            false,
                            new()
                        );
                    }

                    return GetToDoItemParameters(
                        allItems,
                        entity,
                        DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly(),
                        offset,
                        parameters,
                        false,
                        new()
                    );
                }
            );
    }

    private Result<ToDoItemParameters> GetToDoItemParameters(
        FrozenDictionary<Guid, ToDoItemEntity> allItems,
        ToDoItemEntity entity,
        DateOnly dueDate,
        TimeSpan offset,
        ToDoItemParameters parameters,
        bool useDueDate,
        List<Guid> ignoreIds
    )
    {
        logger.LogDebug(
            "ToDoItem: {ToDoItemId} {ToDoItemName} {ToDoItemType} {ToDoItemReferenceId}",
            entity.Id,
            entity.Name,
            entity.Type,
            entity.ReferenceId
        );

        if (entity.Type == ToDoItemType.Reference)
        {
            if (entity.ReferenceId.HasValue && entity.ReferenceId.Value != entity.Id)
            {
                ignoreIds.Add(entity.Id);

                return GetToDoItemParameters(
                    allItems,
                    allItems[entity.ReferenceId.Value],
                    dueDate,
                    offset,
                    parameters,
                    useDueDate,
                    ignoreIds
                );
            }

            return parameters.With(ToDoItemIsCan.None)
               .With(OptionStruct<ToDoShortItem>.Default)
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
            return parameters.With(ToDoItemIsCan.CanIncomplete)
               .With(OptionStruct<ToDoShortItem>.Default)
               .With(ToDoItemStatus.Completed)
               .ToResult();
        }

        var isMiss = false;
        var isDueable = IsDueable(allItems, entity);

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
                    return parameters.With(OptionStruct<ToDoShortItem>.Default)
                       .With(ToDoItemStatus.Planned)
                       .With(ToDoItemIsCan.None)
                       .ToResult();
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
                    return parameters.With(OptionStruct<ToDoShortItem>.Default)
                       .With(ToDoItemStatus.Planned)
                       .With(ToDoItemIsCan.None)
                       .ToResult();
                }
            }
        }

        var items = allItems.Values
           .Where(x => x.ParentId == entity.Id && !ignoreIds.Contains(x.Id))
           .OrderBy(x => x.OrderIndex)
           .ToArray();

        var firstReadyForComplete = new OptionStruct<ToDoShortItem>();
        var firstMiss = new OptionStruct<ToDoShortItem>();
        var hasPlanned = false;

        foreach (var item in items)
        {
            if (firstMiss.IsHasValue)
            {
                break;
            }

            var result = GetToDoItemParameters(
                allItems,
                item,
                dueDate,
                offset,
                parameters,
                true,
                ignoreIds
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
                case ToDoItemStatus.ComingSoon:
                    break;
                default:
                    return new(new ToDoItemStatusOutOfRangeError(parameters.Status));
            }
        }

        var firstActive = firstMiss.IsHasValue ? firstMiss : firstReadyForComplete;
        var isGroupResult = IsGroup(entity);

        if (!isGroupResult.TryGetValue(out var isGroup))
        {
            return new(isGroupResult.Errors);
        }

        if (isGroup)
        {
            if (isMiss)
            {
                return parameters.With(ToDoItemStatus.Miss)
                   .With(ToDoItemIsCan.None)
                   .With(firstActive.IsHasValue ? firstActive : ToActiveToDoItem(entity))
                   .ToResult();
            }

            if (firstMiss.IsHasValue)
            {
                return parameters.With(ToDoItemStatus.Miss).With(ToDoItemIsCan.None).With(firstMiss).ToResult();
            }

            if (firstReadyForComplete.IsHasValue)
            {
                return parameters.With(ToDoItemStatus.ReadyForComplete)
                   .With(ToDoItemIsCan.None)
                   .With(firstReadyForComplete)
                   .ToResult();
            }

            if (hasPlanned)
            {
                return parameters.With(ToDoItemStatus.Planned)
                   .With(ToDoItemIsCan.None)
                   .With(OptionStruct<ToDoShortItem>.Default)
                   .ToResult();
            }

            return parameters.With(ToDoItemStatus.Completed)
               .With(ToDoItemIsCan.None)
               .With(OptionStruct<ToDoShortItem>.Default)
               .ToResult();
        }

        if (isMiss)
        {
            switch (entity.ChildrenType)
            {
                case ToDoItemChildrenType.RequireCompletion:
                    if (firstActive.IsHasValue)
                    {
                        return parameters.With(ToDoItemStatus.Miss)
                           .With(ToDoItemIsCan.None)
                           .With(firstActive)
                           .ToResult();
                    }

                    return parameters.With(ToDoItemStatus.Miss)
                       .With(ToDoItemIsCan.CanComplete)
                       .With(ToActiveToDoItem(entity))
                       .ToResult();
                case ToDoItemChildrenType.IgnoreCompletion:
                    return parameters.With(ToDoItemStatus.Miss)
                       .With(ToDoItemIsCan.CanComplete)
                       .With(firstActive.IsHasValue ? firstActive : ToActiveToDoItem(entity))
                       .ToResult();
                default:
                    return new(new ToDoItemChildrenTypeOutOfRangeError(entity.ChildrenType));
            }
        }

        if (firstMiss.IsHasValue)
        {
            return entity.ChildrenType switch
            {
                ToDoItemChildrenType.RequireCompletion => parameters.With(ToDoItemStatus.Miss)
                   .With(ToDoItemIsCan.None)
                   .With(firstMiss)
                   .ToResult(),
                ToDoItemChildrenType.IgnoreCompletion => parameters.With(ToDoItemStatus.Miss)
                   .With(ToDoItemIsCan.CanComplete)
                   .With(firstMiss)
                   .ToResult(),
                _ => new(new ToDoItemChildrenTypeOutOfRangeError(entity.ChildrenType)),
            };
        }

        if (firstReadyForComplete.IsHasValue)
        {
            return entity.ChildrenType switch
            {
                ToDoItemChildrenType.RequireCompletion => parameters.With(ToDoItemStatus.ReadyForComplete)
                   .With(ToDoItemIsCan.None)
                   .With(firstReadyForComplete)
                   .ToResult(),
                ToDoItemChildrenType.IgnoreCompletion => parameters.With(ToDoItemStatus.ReadyForComplete)
                   .With(ToDoItemIsCan.CanComplete)
                   .With(firstReadyForComplete)
                   .ToResult(),
                _ => new(new ToDoItemChildrenTypeOutOfRangeError(entity.ChildrenType)),
            };
        }

        return parameters.With(ToDoItemStatus.ReadyForComplete)
           .With(ToDoItemIsCan.CanComplete)
           .With(OptionStruct<ToDoShortItem>.Default)
           .ToResult();
    }

    private Result<bool> IsDueable(FrozenDictionary<Guid, ToDoItemEntity> allItems, ToDoItemEntity entity)
    {
        return entity.Type switch
        {
            ToDoItemType.Value => false.ToResult(),
            ToDoItemType.Group => false.ToResult(),
            ToDoItemType.Planned => true.ToResult(),
            ToDoItemType.Periodicity => true.ToResult(),
            ToDoItemType.PeriodicityOffset => true.ToResult(),
            ToDoItemType.Circle => false.ToResult(),
            ToDoItemType.Step => false.ToResult(),
            ToDoItemType.Reference => entity.ReferenceId.HasValue && entity.ReferenceId != entity.Id
                ? IsDueable(allItems, allItems[entity.ReferenceId.Value])
                : false.ToResult(),
            _ => new(new ToDoItemTypeOutOfRangeError(entity.Type)),
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

    private OptionStruct<ToDoShortItem> ToActiveToDoItem(ToDoItemEntity entity)
    {
        return entity.ParentId is null ? OptionStruct<ToDoShortItem>.Default : entity.ToToDoShortItem().ToOption();
    }
}
using Spravy.EventBus.Domain.Errors;
using Spravy.EventBus.Domain.Interfaces;

namespace Spravy.ToDo.Service.Services;

public class EfToDoService : IToDoService
{
    private static readonly ReadOnlyMemory<Guid> eventIds =
        new([AddToDoItemToFavoriteEventOptions.EventId]);

    private readonly IFactory<SpravyDbToDoDbContext> dbContextFactory;
    private readonly GetterToDoItemParametersService getterToDoItemParametersService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IEventBusService eventBusService;
    private readonly ISerializer serializer;

    public EfToDoService(
        IFactory<SpravyDbToDoDbContext> dbContextFactory,
        IHttpContextAccessor httpContextAccessor,
        GetterToDoItemParametersService getterToDoItemParametersService,
        IEventBusService eventBusService,
        ISerializer serializer
    )
    {
        this.dbContextFactory = dbContextFactory;
        this.httpContextAccessor = httpContextAccessor;
        this.getterToDoItemParametersService = getterToDoItemParametersService;
        this.eventBusService = eventBusService;
        this.serializer = serializer;
    }

    public ConfiguredValueTaskAwaitable<Result<Guid>> CloneToDoItemAsync(
        Guid cloneId,
        OptionStruct<Guid> parentId,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(cloneId)
                                .IfSuccessAsync(
                                    clone =>
                                        AddCloneAsync(context, clone, parentId, ct)
                                            .ConfigureAwait(false),
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDescriptionTypeAsync(
        Guid id,
        DescriptionType type,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.DescriptionType = type;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<
        Result<OptionStruct<ActiveToDoItem>>
    > GetActiveToDoItemAsync(Guid id, CancellationToken ct)
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();

        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                        getterToDoItemParametersService.GetToDoItemParametersAsync(
                                            context,
                                            item,
                                            offset,
                                            ct
                                        ),
                                    ct
                                )
                                .IfSuccessAsync(parameters => parameters.ActiveItem.ToResult(), ct),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateReferenceToDoItemAsync(
        Guid id,
        Guid referenceId,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.ReferenceId = referenceId;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ResetToDoItemAsync(
        ResetToDoItemOptions options,
        CancellationToken ct
    )
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();

        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(options.Id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        if (options.IsCompleteCurrentTask)
                                        {
                                            item.IsCompleted = true;

                                            return UpdateDueDateAsync(context, item, offset, ct)
                                                .IfSuccessAsync(item.ToResult, ct);
                                        }

                                        return item.ToResult()
                                            .ToValueTaskResult()
                                            .ConfigureAwait(false);
                                    },
                                    ct
                                )
                                .IfSuccessAsync(
                                    item =>
                                        CircleCompletionAsync(
                                                context,
                                                item,
                                                options.IsMoveCircleOrderIndex,
                                                options.IsCompleteChildrenTask,
                                                options.IsOnlyCompletedTasks,
                                                ct
                                            )
                                            .IfSuccessAsync(
                                                () =>
                                                    StepCompletionAsync(
                                                        context,
                                                        item,
                                                        options.IsCompleteChildrenTask,
                                                        ct
                                                    ),
                                                ct
                                            ),
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> UpdateEventsAsync(CancellationToken ct)
    {
        return eventBusService
            .GetEventsAsync(eventIds, ct)
            .IfSuccessAsync(
                events =>
                {
                    if (events.IsEmpty)
                    {
                        return false.ToResult().ToValueTaskResult().ConfigureAwait(false);
                    }

                    return dbContextFactory
                        .Create()
                        .IfSuccessDisposeAsync(
                            context =>
                                context.AtomicExecuteAsync(
                                    () =>
                                        events.IfSuccessForEachAsync(
                                            e =>
                                            {
                                                if (
                                                    e.Id
                                                    == AddToDoItemToFavoriteEventOptions.EventId
                                                )
                                                {
                                                    return serializer
                                                        .DeserializeAsync<AddToDoItemToFavoriteEventOptions>(
                                                            e.Content,
                                                            ct
                                                        )
                                                        .IfSuccessAsync(
                                                            options =>
                                                                context
                                                                    .GetEntityAsync<ToDoItemEntity>(
                                                                        options.ToDoItemId
                                                                    )
                                                                    .IfSuccessAsync(
                                                                        x =>
                                                                        {
                                                                            x.IsFavorite = true;

                                                                            return Result.Success;
                                                                        },
                                                                        ct
                                                                    ),
                                                            ct
                                                        );
                                                }

                                                return new Result(new NotFoundEventError(e.Id))
                                                    .ToValueTaskResult()
                                                    .ConfigureAwait(false);
                                            },
                                            ct
                                        ),
                                    ct
                                ),
                            ct
                        )
                        .IfSuccessAsync(
                            () => true.ToResult().ToValueTaskResult().ConfigureAwait(false),
                            ct
                        );
                },
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> RandomizeChildrenOrderIndexAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .Set<ToDoItemEntity>()
                                .Where(x => x.ParentId == id)
                                .ToArrayEntitiesAsync(ct)
                                .IfSuccessAsync(
                                    children =>
                                    {
                                        SetRandomOrderIndex(children);

                                        var items = children.Span.OrderBy(x => x.OrderIndex);

                                        for (var i = 0; items.Length > i; i++)
                                        {
                                            items[i].OrderIndex = (uint)i;
                                        }

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    private void SetRandomOrderIndex(ReadOnlyMemory<ToDoItemEntity> entities)
    {
        foreach (var entity in entities.Span)
        {
            entity.OrderIndex = (uint)RandomNumberGenerator.GetInt32(0, Int32.MaxValue);
        }
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .GetEntityAsync<ToDoItemEntity>(id)
                        .IfSuccessAsync(
                            item =>
                            {
                                var parents = new List<ToDoShortItem> { new(item.Id, item.Name) };

                                return GetParentsAsync(context, id, parents, ct)
                                    .ConfigureAwait(false)
                                    .IfSuccessAsync(
                                        () =>
                                        {
                                            parents.Reverse();

                                            return parents.ToArray().ToReadOnlyMemory().ToResult();
                                        },
                                        ct
                                    );
                            },
                            ct
                        ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsAsync(
        string searchText,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .Set<ToDoItemEntity>()
                        .AsNoTracking()
                        .Where(x => x.Name.Contains(searchText))
                        .OrderBy(x => x.OrderIndex)
                        .Select(x => x.Id)
                        .ToArrayEntitiesAsync(ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .Set<ToDoItemEntity>()
                        .AsNoTracking()
                        .Where(x => x.ParentId == id)
                        .OrderBy(x => x.OrderIndex)
                        .ToArrayEntitiesAsync(ct)
                        .IfSuccessAsync(
                            entities =>
                            {
                                if (entities.IsEmpty)
                                {
                                    return ReadOnlyMemory<Guid>
                                        .Empty.ToResult()
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false);
                                }

                                var result = new List<Guid>();

                                return entities
                                    .ToResult()
                                    .IfSuccessForEachAsync(
                                        e =>
                                            GetLeafToDoItemIdsAsync(context, e, new(), ct)
                                                .ConfigureAwait(false)
                                                .IfSuccessForEachAsync(
                                                    i =>
                                                    {
                                                        result.Add(i);

                                                        return Result.Success;
                                                    },
                                                    ct
                                                ),
                                        ct
                                    )
                                    .IfSuccessAsync(
                                        () => result.ToArray().ToReadOnlyMemory().ToResult(),
                                        ct
                                    );
                            },
                            ct
                        ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<FullToDoItem>> GetToDoItemAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .GetEntityAsync<ToDoItemEntity>(id)
                        .IfSuccessAsync(
                            item =>
                                getterToDoItemParametersService
                                    .GetToDoItemParametersAsync(
                                        context,
                                        item,
                                        httpContextAccessor
                                            .HttpContext.ThrowIfNull()
                                            .GetTimeZoneOffset(),
                                        ct
                                    )
                                    .IfSuccessAsync(
                                        parameters =>
                                        {
                                            if (
                                                item.Type == ToDoItemType.Reference
                                                && item.ReferenceId.HasValue
                                            )
                                            {
                                                return context
                                                    .GetEntityAsync<ToDoItemEntity>(
                                                        item.ReferenceId.Value
                                                    )
                                                    .IfSuccessAsync(
                                                        i =>
                                                            (
                                                                i with
                                                                {
                                                                    Id = item.Id,
                                                                    ReferenceId = item.ReferenceId,
                                                                    ParentId = item.ParentId,
                                                                    Type = ToDoItemType.Reference,
                                                                    OrderIndex = item.OrderIndex,
                                                                    Name = item.Name,
                                                                }
                                                            )
                                                                .ToFullToDoItem(parameters)
                                                                .ToResult(),
                                                        ct
                                                    );
                                            }

                                            return item.ToFullToDoItem(parameters)
                                                .ToResult()
                                                .ToValueTaskResult()
                                                .ConfigureAwait(false);
                                        },
                                        ct
                                    ),
                            ct
                        ),
                ct
            );
    }

    private ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoItem>>> GetToDoItemsAsync(
        ReadOnlyMemory<Guid> ids,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .Set<ToDoItemEntity>()
                        .AsNoTracking()
                        .Where(x => ids.ToArray().Contains(x.Id))
                        .OrderBy(x => x.OrderIndex)
                        .ToArrayEntitiesAsync(ct)
                        .IfSuccessForEachAsync(
                            item =>
                                getterToDoItemParametersService
                                    .GetToDoItemParametersAsync(
                                        context,
                                        item,
                                        httpContextAccessor
                                            .HttpContext.ThrowIfNull()
                                            .GetTimeZoneOffset(),
                                        ct
                                    )
                                    .IfSuccessAsync(
                                        parameters =>
                                        {
                                            if (
                                                item
                                                    is {
                                                        Type: ToDoItemType.Reference,
                                                        ReferenceId: not null
                                                    }
                                                && item.ReferenceId != item.Id
                                            )
                                            {
                                                return context
                                                    .GetEntityAsync<ToDoItemEntity>(
                                                        item.ReferenceId.Value
                                                    )
                                                    .IfSuccessAsync(
                                                        i =>
                                                            (
                                                                i with
                                                                {
                                                                    Id = item.Id,
                                                                    ReferenceId = item.ReferenceId,
                                                                    ParentId = item.ParentId,
                                                                    Type = ToDoItemType.Reference,
                                                                    OrderIndex = item.OrderIndex,
                                                                    Name = item.Name,
                                                                }
                                                            )
                                                                .ToToDoItem(parameters)
                                                                .ToResult(),
                                                        ct
                                                    );
                                            }

                                            return item.ToToDoItem(parameters)
                                                .ToResult()
                                                .ToValueTaskResult()
                                                .ConfigureAwait(false);
                                        },
                                        ct
                                    ),
                            ct
                        ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .Set<ToDoItemEntity>()
                        .AsNoTracking()
                        .Where(x => x.ParentId == id)
                        .OrderBy(x => x.OrderIndex)
                        .Select(x => x.Id)
                        .ToArrayEntitiesAsync(ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<ToDoShortItem>>
    > GetChildrenToDoItemShortsAsync(Guid id, CancellationToken ct)
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .Set<ToDoItemEntity>()
                        .AsNoTracking()
                        .Where(x => x.ParentId == id)
                        .OrderBy(x => x.OrderIndex)
                        .ToArrayEntitiesAsync(ct)
                        .IfSuccessAsync(items => items.ToToDoShortItem().ToResult(), ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetRootToDoItemIdsAsync(
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .Set<ToDoItemEntity>()
                        .AsNoTracking()
                        .Where(x => x.ParentId == null)
                        .OrderBy(x => x.OrderIndex)
                        .Select(x => x.Id)
                        .ToArrayEntitiesAsync(ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .Set<ToDoItemEntity>()
                        .AsNoTracking()
                        .OrderBy(x => x.OrderIndex)
                        .Where(x => x.IsFavorite)
                        .Select(x => x.Id)
                        .ToArrayEntitiesAsync(ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<Guid>> AddToDoItemAsync(
        AddToDoItemOptions options,
        CancellationToken ct
    )
    {
        var parentId = options.ParentId.GetValueOrNull();
        var id = Guid.NewGuid();
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();

        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                        {
                            if (parentId is not null)
                            {
                                return context
                                    .GetEntityAsync<ToDoItemEntity>(parentId)
                                    .IfSuccessAsync(
                                        parent =>
                                            context
                                                .Set<ToDoItemEntity>()
                                                .AsNoTracking()
                                                .Where(x => x.ParentId == parentId)
                                                .Select(x => x.OrderIndex)
                                                .ToArrayEntitiesAsync(ct)
                                                .IfSuccessAsync(
                                                    items =>
                                                    {
                                                        var toDoItem = options.ToToDoItemEntity();
                                                        toDoItem.Description = options.Description;
                                                        toDoItem.Id = id;
                                                        toDoItem.OrderIndex =
                                                            items.Length == 0 ? 0 : items.Max() + 1;
                                                        toDoItem.DueDate =
                                                            parent.DueDate
                                                            < DateTimeOffset
                                                                .UtcNow.Add(offset)
                                                                .Date.ToDateOnly()
                                                                ? DateTimeOffset
                                                                    .UtcNow.Add(offset)
                                                                    .Date.ToDateOnly()
                                                                : parent.DueDate;
                                                        toDoItem.TypeOfPeriodicity =
                                                            parent.TypeOfPeriodicity;
                                                        toDoItem.DaysOfMonth = parent.DaysOfMonth;
                                                        toDoItem.DaysOfWeek = parent.DaysOfWeek;
                                                        toDoItem.DaysOfYear = parent.DaysOfYear;
                                                        toDoItem.WeeksOffset = parent.WeeksOffset;
                                                        toDoItem.DaysOffset = parent.DaysOffset;
                                                        toDoItem.MonthsOffset = parent.MonthsOffset;
                                                        toDoItem.YearsOffset = parent.YearsOffset;
                                                        toDoItem.Link = options.Link.TryGetValue(
                                                            out var uri
                                                        )
                                                            ? uri.AbsoluteUri
                                                            : string.Empty;
                                                        toDoItem.DescriptionType =
                                                            options.DescriptionType;

                                                        return context
                                                            .AddEntityAsync(toDoItem, ct)
                                                            .IfSuccessAsync(_ => id.ToResult(), ct);
                                                    },
                                                    ct
                                                ),
                                        ct
                                    );
                            }

                            return context
                                .Set<ToDoItemEntity>()
                                .AsNoTracking()
                                .Where(x => x.ParentId == parentId)
                                .Select(x => x.OrderIndex)
                                .ToArrayEntitiesAsync(ct)
                                .IfSuccessAsync(
                                    items =>
                                    {
                                        var toDoItem = options.ToToDoItemEntity();
                                        toDoItem.Description = options.Description;
                                        toDoItem.Id = id;
                                        toDoItem.OrderIndex =
                                            items.Length == 0 ? 0 : items.Max() + 1;

                                        toDoItem.DueDate = DateTimeOffset
                                            .UtcNow.Add(offset)
                                            .Date.ToDateOnly();

                                        toDoItem.Link = options.Link.TryGetValue(out var uri)
                                            ? uri.AbsoluteUri
                                            : string.Empty;

                                        toDoItem.DescriptionType = options.DescriptionType;

                                        return context
                                            .AddEntityAsync(toDoItem, ct)
                                            .IfSuccessAsync(_ => id.ToResult(), ct);
                                    },
                                    ct
                                );
                        },
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteToDoItemAsync(Guid id, CancellationToken ct)
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                        DeleteToDoItemAsync(id, context, ct)
                                            .IfSuccessAsync(
                                                () =>
                                                    NormalizeOrderIndexAsync(
                                                        context,
                                                        item.ParentId.ToOption(),
                                                        ct
                                                    ),
                                                ct
                                            ),
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemTypeOfPeriodicityAsync(
        Guid id,
        TypeOfPeriodicity type,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.TypeOfPeriodicity = type;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDueDateAsync(
        Guid id,
        DateOnly dueDate,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.DueDate = dueDate;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemCompleteStatusAsync(
        Guid id,
        bool isComplete,
        CancellationToken ct
    )
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();

        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        if (isComplete)
                                        {
                                            return getterToDoItemParametersService
                                                .GetToDoItemParametersAsync(
                                                    context,
                                                    item,
                                                    offset,
                                                    ct
                                                )
                                                .IfSuccessAsync(
                                                    parameters =>
                                                    {
                                                        if (
                                                            !parameters.IsCan.HasFlag(
                                                                ToDoItemIsCan.CanComplete
                                                            )
                                                        )
                                                        {
                                                            return new Result(
                                                                new ToDoItemAlreadyCompleteError(
                                                                    item.Id,
                                                                    item.Name
                                                                )
                                                            )
                                                                .ToValueTaskResult()
                                                                .ConfigureAwait(false);
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
                                                            default:
                                                                throw new ArgumentOutOfRangeException();
                                                        }

                                                        return UpdateDueDateAsync(
                                                                context,
                                                                item,
                                                                offset,
                                                                ct
                                                            )
                                                            .IfSuccessAsync(
                                                                () =>
                                                                {
                                                                    item.LastCompleted =
                                                                        DateTimeOffset.Now;

                                                                    return CircleCompletionAsync(
                                                                            context,
                                                                            item,
                                                                            true,
                                                                            false,
                                                                            false,
                                                                            ct
                                                                        )
                                                                        .IfSuccessAsync(
                                                                            () =>
                                                                                StepCompletionAsync(
                                                                                    context,
                                                                                    item,
                                                                                    false,
                                                                                    ct
                                                                                ),
                                                                            ct
                                                                        );
                                                                },
                                                                ct
                                                            );
                                                    },
                                                    ct
                                                );
                                        }

                                        item.IsCompleted = false;

                                        return Result.AwaitableSuccess;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemNameAsync(
        Guid id,
        string name,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.Name = name;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemOrderIndexAsync(
        UpdateOrderIndexToDoItemOptions options,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(options.Id)
                                .IfSuccessAsync(
                                    item =>
                                        context
                                            .GetEntityAsync<ToDoItemEntity>(options.TargetId)
                                            .IfSuccessAsync(
                                                targetItem =>
                                                {
                                                    var orderIndex = options.IsAfter
                                                        ? targetItem.OrderIndex + 1
                                                        : targetItem.OrderIndex;

                                                    return context
                                                        .Set<ToDoItemEntity>()
                                                        .Where(x =>
                                                            x.ParentId == item.ParentId
                                                            && x.Id != item.Id
                                                            && x.OrderIndex >= orderIndex
                                                        )
                                                        .ToArrayEntitiesAsync(ct)
                                                        .IfSuccessAsync(
                                                            items =>
                                                            {
                                                                foreach (
                                                                    var itemEntity in items.Span
                                                                )
                                                                {
                                                                    itemEntity.OrderIndex++;
                                                                }

                                                                item.OrderIndex = orderIndex;

                                                                return NormalizeOrderIndexAsync(
                                                                    context,
                                                                    item.ParentId.ToOption(),
                                                                    ct
                                                                );
                                                            },
                                                            ct
                                                        );
                                                },
                                                ct
                                            ),
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDescriptionAsync(
        Guid id,
        string description,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .Set<ToDoItemEntity>()
                                .Where(x => x.Id == id)
                                .ExecuteUpdateEntityAsync(
                                    x => x.SetProperty(y => y.Description, description),
                                    ct
                                )
                                .ToResultOnlyAsync(),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemTypeAsync(
        Guid id,
        ToDoItemType type,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.Type = type;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> AddFavoriteToDoItemAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.IsFavorite = true;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> RemoveFavoriteToDoItemAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.IsFavorite = false;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemIsRequiredCompleteInDueDateAsync(
        Guid id,
        bool value,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.IsRequiredCompleteInDueDate = value;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(
        CancellationToken ct
    )
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();

        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .Set<ToDoItemEntity>()
                        .AsNoTracking()
                        .Where(x =>
                            !x.IsCompleted
                            && (
                                x.Type == ToDoItemType.Periodicity
                                || x.Type == ToDoItemType.PeriodicityOffset
                                || x.Type == ToDoItemType.Planned
                            )
                        )
                        .Select(x => new { x.Id, x.DueDate })
                        .ToArrayEntitiesAsync(ct)
                        .IfSuccessAsync(
                            items =>
                                items
                                    .ToArray()
                                    .Where(x =>
                                        x.DueDate
                                        <= DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly()
                                    )
                                    .Select(x => x.Id)
                                    .ToArray()
                                    .ToReadOnlyMemory()
                                    .ToResult(),
                            ct
                        ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemAnnuallyPeriodicityAsync(
        Guid id,
        AnnuallyPeriodicity periodicity,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.SetDaysOfYear(periodicity.Days);

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemMonthlyPeriodicityAsync(
        Guid id,
        MonthlyPeriodicity periodicity,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.SetDaysOfMonth(periodicity.Days);

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemWeeklyPeriodicityAsync(
        Guid id,
        WeeklyPeriodicity periodicity,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.SetDaysOfWeek(periodicity.Days);

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<ToDoSelectorItem>>
    > GetToDoSelectorItemsAsync(ReadOnlyMemory<Guid> ignoreIds, CancellationToken ct)
    {
        var list = new List<Guid>(ignoreIds.ToArray());

        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .Set<ToDoItemEntity>()
                        .AsNoTracking()
                        .Where(x => !list.Contains(x.Id) && x.Type != ToDoItemType.Reference)
                        .OrderBy(x => x.OrderIndex)
                        .ToArrayEntitiesAsync(ct)
                        .IfSuccessAsync(
                            items =>
                                items
                                    .Where(x => x.ParentId is null)
                                    .ToResult()
                                    .IfSuccessForEach(x =>
                                        GetToDoSelectorItems(items, x.Id)
                                            .IfSuccess(children =>
                                                new ToDoSelectorItem(
                                                    x.Id,
                                                    x.Name,
                                                    x.OrderIndex,
                                                    children
                                                ).ToResult()
                                            )
                                    ),
                            ct
                        ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemParentAsync(
        Guid id,
        Guid parentId,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    entity =>
                                        context
                                            .Set<ToDoItemEntity>()
                                            .AsNoTracking()
                                            .Where(x => x.ParentId == parentId)
                                            .Select(x => x.OrderIndex)
                                            .ToArrayEntitiesAsync(ct)
                                            .IfSuccessAsync(
                                                items =>
                                                {
                                                    entity = entity.ThrowIfNull();
                                                    entity.ParentId = parentId;
                                                    entity.OrderIndex =
                                                        items.Length == 0 ? 0 : items.Max() + 1;

                                                    return Result.Success;
                                                },
                                                ct
                                            ),
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ToDoItemToRootAsync(Guid id, CancellationToken ct)
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    entity =>
                                        context
                                            .Set<ToDoItemEntity>()
                                            .AsNoTracking()
                                            .Where(x => x.ParentId == null)
                                            .Select(x => x.OrderIndex)
                                            .ToArrayEntitiesAsync(ct)
                                            .IfSuccessAsync(
                                                items =>
                                                {
                                                    entity = entity.ThrowIfNull();
                                                    entity.ParentId = null;
                                                    entity.OrderIndex =
                                                        items.Length == 0 ? 0 : items.Max() + 1;

                                                    return Result.Success;
                                                },
                                                ct
                                            ),
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<string>> ToDoItemToStringAsync(
        ToDoItemToStringOptions options,
        CancellationToken ct
    )
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();

        return dbContextFactory
            .Create()
            .IfSuccessAsync(
                context =>
                {
                    var builder = new StringBuilder();

                    return ToDoItemToStringAsync(context, options, 0, builder, offset, ct)
                        .IfSuccessAsync(() => builder.ToString().Trim().ToResult(), ct);
                },
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDaysOffsetAsync(
        Guid id,
        ushort days,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.DaysOffset = days;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemMonthsOffsetAsync(
        Guid id,
        ushort months,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.MonthsOffset = months;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemWeeksOffsetAsync(
        Guid id,
        ushort weeks,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.WeeksOffset = weeks;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemYearsOffsetAsync(
        Guid id,
        ushort years,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.YearsOffset = years;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemChildrenTypeAsync(
        Guid id,
        ToDoItemChildrenType type,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.ChildrenType = type;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetSiblingsAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .GetEntityAsync<ToDoItemEntity>(id)
                        .IfSuccessAsync(
                            item =>
                                context
                                    .Set<ToDoItemEntity>()
                                    .Where(x => x.ParentId == item.ParentId && x.Id != item.Id)
                                    .OrderBy(x => x.OrderIndex)
                                    .ToArrayEntitiesAsync(ct)
                                    .IfSuccessAsync(
                                        items => items.ToToDoShortItem().ToResult(),
                                        ct
                                    ),
                            ct
                        ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<
        Result<OptionStruct<ActiveToDoItem>>
    > GetCurrentActiveToDoItemAsync(CancellationToken ct)
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();

        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .Set<ToDoItemEntity>()
                        .Where(x => x.ParentId == null)
                        .OrderBy(x => x.OrderIndex)
                        .ToArrayEntitiesAsync(ct)
                        .IfSuccessForEachAsync(
                            item =>
                                getterToDoItemParametersService
                                    .GetToDoItemParametersAsync(context, item, offset, ct)
                                    .IfSuccessAsync(
                                        parameters =>
                                        {
                                            if (parameters.ActiveItem.IsHasValue)
                                            {
                                                return parameters.ActiveItem.ToResult();
                                            }

                                            return new(new OptionStruct<ActiveToDoItem>());
                                        },
                                        ct
                                    ),
                            ct
                        )
                        .IfSuccessAsync(
                            items =>
                            {
                                var item = items.Span.FirstOrDefault(x => x.IsHasValue);

                                return new Result<OptionStruct<ActiveToDoItem>>(item);
                            },
                            ct
                        ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemLinkAsync(
        Guid id,
        Option<Uri> link,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<ToDoItemEntity>(id)
                                .IfSuccessAsync(
                                    value =>
                                    {
                                        value.Link = link.MapToString();

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<
        Result<PlannedToDoItemSettings>
    > GetPlannedToDoItemSettingsAsync(Guid id, CancellationToken ct)
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .GetEntityAsync<ToDoItemEntity>(id)
                        .IfSuccessAsync(item => item.ToPlannedToDoItemSettings().ToResult(), ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<
        Result<ValueToDoItemSettings>
    > GetValueToDoItemSettingsAsync(Guid id, CancellationToken ct)
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .GetEntityAsync<ToDoItemEntity>(id)
                        .IfSuccessAsync(item => item.ToValueToDoItemSettings().ToResult(), ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<
        Result<PeriodicityToDoItemSettings>
    > GetPeriodicityToDoItemSettingsAsync(Guid id, CancellationToken ct)
    {
        return dbContextFactory
            .Create()
            .IfSuccessAsync(
                context =>
                    context
                        .GetEntityAsync<ToDoItemEntity>(id)
                        .IfSuccessAsync(
                            item => item.ToPeriodicityToDoItemSettings().ToResult(),
                            ct
                        ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<WeeklyPeriodicity>> GetWeeklyPeriodicityAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .GetEntityAsync<ToDoItemEntity>(id)
                        .IfSuccessAsync(item => item.ToWeeklyPeriodicity().ToResult(), ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<MonthlyPeriodicity>> GetMonthlyPeriodicityAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .GetEntityAsync<ToDoItemEntity>(id)
                        .IfSuccessAsync(item => item.ToMonthlyPeriodicity().ToResult(), ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<AnnuallyPeriodicity>> GetAnnuallyPeriodicityAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .GetEntityAsync<ToDoItemEntity>(id)
                        .IfSuccessAsync(item => item.ToAnnuallyPeriodicity().ToResult(), ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<
        Result<PeriodicityOffsetToDoItemSettings>
    > GetPeriodicityOffsetToDoItemSettingsAsync(Guid id, CancellationToken ct)
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .GetEntityAsync<ToDoItemEntity>(id)
                        .IfSuccessAsync(
                            item => item.ToPeriodicityOffsetToDoItemSettings().ToResult(),
                            ct
                        ),
                ct
            );
    }

    public ConfiguredCancelableAsyncEnumerable<Result<ReadOnlyMemory<ToDoItem>>> GetToDoItemsAsync(
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        CancellationToken ct
    )
    {
        return GetToDoItemsCore(ids, chunkSize, ct).ConfigureAwait(false);
    }

    private async IAsyncEnumerable<Result<ReadOnlyMemory<ToDoItem>>> GetToDoItemsCore(
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        [EnumeratorCancellation] CancellationToken ct
    )
    {
        if (ids.IsEmpty)
        {
            yield break;
        }

        for (uint i = 0; i < ids.Length; i += chunkSize)
        {
            var size = i + chunkSize > ids.Length ? (int)(ids.Length - i) : (int)chunkSize;
            var range = ids.Slice((int)i, size);

            if (range.IsEmpty)
            {
                yield break;
            }

            var items = await GetToDoItemsAsync(range, ct);

            if (items.IsHasError)
            {
                yield return items;

                yield break;
            }

            yield return items;
        }
    }

    private async ValueTask<Result<Guid>> AddCloneAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity clone,
        OptionStruct<Guid> parentId,
        CancellationToken ct
    )
    {
        var id = clone.Id;
        clone.Id = Guid.NewGuid();
        clone.ParentId = parentId.TryGetValue(out var value) ? value : null;
        await context.AddAsync(clone, ct);

        var items = await context
            .Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == id)
            .ToArrayAsync(ct);

        foreach (var item in items)
        {
            await AddCloneAsync(context, item, clone.Id.ToOption(), ct);
        }

        return clone.Id.ToResult();
    }

    private ConfiguredValueTaskAwaitable<Result> DeleteToDoItemAsync(
        Guid id,
        SpravyDbToDoDbContext context,
        CancellationToken ct
    )
    {
        return context
            .GetEntityAsync<ToDoItemEntity>(id)
            .IfSuccessAsync(
                item =>
                    context
                        .Set<ToDoItemEntity>()
                        .AsNoTracking()
                        .Where(x => x.ParentId == id)
                        .ToArrayEntitiesAsync(ct)
                        .IfSuccessForEachAsync(
                            child => DeleteToDoItemAsync(child.Id, context, ct),
                            ct
                        )
                        .IfSuccessAsync(
                            () =>
                                context
                                    .Set<ToDoItemEntity>()
                                    .Where(x => x.ReferenceId == id)
                                    .ToArrayEntitiesAsync(ct)
                                    .IfSuccessForEachAsync(
                                        i =>
                                        {
                                            i.ReferenceId = null;

                                            return Result.AwaitableSuccess;
                                        },
                                        ct
                                    ),
                            ct
                        )
                        .IfSuccessAsync(() => context.RemoveEntity(item), ct),
                ct
            );
    }

    private ConfiguredValueTaskAwaitable<Result> StepCompletionAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity item,
        bool completeTask,
        CancellationToken ct
    )
    {
        return context
            .Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Step)
            .OrderBy(x => x.OrderIndex)
            .ToArrayEntitiesAsync(ct)
            .IfSuccessAsync(
                steps =>
                {
                    foreach (var step in steps.Span)
                    {
                        step.IsCompleted = completeTask;
                    }

                    return context
                        .Set<ToDoItemEntity>()
                        .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Group)
                        .OrderBy(x => x.OrderIndex)
                        .ToArrayEntitiesAsync(ct)
                        .IfSuccessForEachAsync(
                            group => StepCompletionAsync(context, group, completeTask, ct),
                            ct
                        );
                },
                ct
            )
            .IfSuccessAsync(
                () =>
                    context
                        .Set<ToDoItemEntity>()
                        .Where(x =>
                            x.ParentId == item.Id
                            && x.Type == ToDoItemType.Reference
                            && x.ReferenceId.HasValue
                        )
                        .OrderBy(x => x.OrderIndex)
                        .ToArrayEntitiesAsync(ct)
                        .IfSuccessForEachAsync(
                            reference =>
                                context
                                    .GetEntityAsync<ToDoItemEntity>(
                                        reference.ReferenceId.ThrowIfNullStruct()
                                    )
                                    .IfSuccessAsync(
                                        i =>
                                            i.Type switch
                                            {
                                                ToDoItemType.Value => Result.AwaitableSuccess,
                                                ToDoItemType.Group => StepCompletionAsync(
                                                    context,
                                                    i,
                                                    completeTask,
                                                    ct
                                                ),
                                                ToDoItemType.Planned => Result.AwaitableSuccess,
                                                ToDoItemType.Periodicity => Result.AwaitableSuccess,
                                                ToDoItemType.PeriodicityOffset =>
                                                    Result.AwaitableSuccess,
                                                ToDoItemType.Circle => Result.AwaitableSuccess,
                                                ToDoItemType.Step => Result
                                                    .Execute(() => i.IsCompleted = completeTask)
                                                    .ToValueTaskResult()
                                                    .ConfigureAwait(false),
                                                ToDoItemType.Reference => new Result(
                                                    new ToDoItemTypeOutOfRangeError(i.Type)
                                                )
                                                    .ToValueTaskResult()
                                                    .ConfigureAwait(false),
                                                _ => new Result(
                                                    new ToDoItemTypeOutOfRangeError(i.Type)
                                                )
                                                    .ToValueTaskResult()
                                                    .ConfigureAwait(false),
                                            },
                                        ct
                                    ),
                            ct
                        ),
                ct
            );
    }

    private ConfiguredValueTaskAwaitable<Result> CircleCompletionAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity item,
        bool moveCircleOrderIndex,
        bool completeTask,
        bool onlyCompletedTasks,
        CancellationToken ct
    )
    {
        return context
            .Set<ToDoItemEntity>()
            .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Circle)
            .OrderBy(x => x.OrderIndex)
            .ToArrayEntitiesAsync(ct)
            .IfSuccessAsync(
                circleChildren =>
                {
                    if (circleChildren.Length != 0)
                    {
                        if (!onlyCompletedTasks || circleChildren.Span.All(x => x.IsCompleted))
                        {
                            var nextOrderIndex = item.CurrentCircleOrderIndex;

                            if (moveCircleOrderIndex)
                            {
                                var next = circleChildren.Span.FirstOrDefault(x =>
                                    x.OrderIndex > item.CurrentCircleOrderIndex
                                );

                                nextOrderIndex =
                                    next?.OrderIndex ?? circleChildren.Span[0].OrderIndex;
                                item.CurrentCircleOrderIndex = nextOrderIndex;
                            }

                            foreach (var child in circleChildren.Span)
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

                    return context
                        .Set<ToDoItemEntity>()
                        .Where(x => x.ParentId == item.Id && x.Type == ToDoItemType.Group)
                        .OrderBy(x => x.OrderIndex)
                        .ToArrayEntitiesAsync(ct)
                        .IfSuccessForEachAsync(
                            group =>
                                CircleCompletionAsync(
                                    context,
                                    group,
                                    moveCircleOrderIndex,
                                    completeTask,
                                    onlyCompletedTasks,
                                    ct
                                ),
                            ct
                        );
                },
                ct
            )
            .IfSuccessAsync(
                () =>
                    context
                        .Set<ToDoItemEntity>()
                        .Where(x =>
                            x.ParentId == item.Id
                            && x.Type == ToDoItemType.Reference
                            && x.ReferenceId.HasValue
                        )
                        .OrderBy(x => x.OrderIndex)
                        .ToArrayEntitiesAsync(ct)
                        .IfSuccessForEachAsync(
                            reference =>
                                context
                                    .GetEntityAsync<ToDoItemEntity>(
                                        reference.ReferenceId.ThrowIfNullStruct()
                                    )
                                    .IfSuccessAsync(
                                        i =>
                                            i.Type switch
                                            {
                                                ToDoItemType.Value => Result.AwaitableSuccess,
                                                ToDoItemType.Group => CircleCompletionAsync(
                                                    context,
                                                    i,
                                                    moveCircleOrderIndex,
                                                    completeTask,
                                                    onlyCompletedTasks,
                                                    ct
                                                ),
                                                ToDoItemType.Planned => Result.AwaitableSuccess,
                                                ToDoItemType.Periodicity => Result.AwaitableSuccess,
                                                ToDoItemType.PeriodicityOffset =>
                                                    Result.AwaitableSuccess,
                                                ToDoItemType.Circle => Result.AwaitableSuccess,
                                                ToDoItemType.Step => Result.AwaitableSuccess,
                                                ToDoItemType.Reference => new Result(
                                                    new ToDoItemTypeOutOfRangeError(i.Type)
                                                )
                                                    .ToValueTaskResult()
                                                    .ConfigureAwait(false),
                                                _ => new Result(
                                                    new ToDoItemTypeOutOfRangeError(i.Type)
                                                )
                                                    .ToValueTaskResult()
                                                    .ConfigureAwait(false),
                                            },
                                        ct
                                    ),
                            ct
                        ),
                ct
            );
    }

    private ConfiguredValueTaskAwaitable<Result> ToDoItemToStringAsync(
        SpravyDbToDoDbContext context,
        ToDoItemToStringOptions options,
        ushort level,
        StringBuilder builder,
        TimeSpan offset,
        CancellationToken ct
    )
    {
        return context
            .Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == options.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayEntitiesAsync(ct)
            .IfSuccessForEachAsync(
                item =>
                    getterToDoItemParametersService
                        .GetToDoItemParametersAsync(context, item, offset, ct)
                        .IfSuccessAsync(
                            parameters =>
                            {
                                if (
                                    !options
                                        .Statuses.Select(x => (byte)x)
                                        .Span.Contains((byte)parameters.Status)
                                )
                                {
                                    return Result.AwaitableSuccess;
                                }

                                builder.Duplicate(" ", level);
                                builder.Append(item.Name);
                                builder.AppendLine();

                                return ToDoItemToStringAsync(
                                    context,
                                    new(options.Statuses, item.Id),
                                    (ushort)(level + 1),
                                    builder,
                                    offset,
                                    ct
                                );
                            },
                            ct
                        ),
                ct
            );
    }

    private Result<ReadOnlyMemory<ToDoSelectorItem>> GetToDoSelectorItems(
        ReadOnlyMemory<ToDoItemEntity> items,
        Guid id
    )
    {
        return items
            .Where(x => x.ParentId == id)
            .OrderBy(x => x.OrderIndex)
            .IfSuccessForEach(item =>
                GetToDoSelectorItems(items, item.Id)
                    .IfSuccess(children =>
                        new ToDoSelectorItem(
                            item.Id,
                            item.Name,
                            item.OrderIndex,
                            children
                        ).ToResult()
                    )
            );
    }

    private async IAsyncEnumerable<Result<Guid>> GetLeafToDoItemIdsAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity itemEntity,
        List<Guid> ignoreIds,
        [EnumeratorCancellation] CancellationToken ct
    )
    {
        if (ignoreIds.Contains(itemEntity.Id))
        {
            yield break;
        }

        if (itemEntity.Type == ToDoItemType.Reference)
        {
            ignoreIds.Add(itemEntity.Id);

            if (itemEntity.ReferenceId is null)
            {
                yield return itemEntity.Id.ToResult();

                yield break;
            }

            var result = await context.GetEntityAsync<ToDoItemEntity>(itemEntity.ReferenceId);

            if (!result.TryGetValue(out var reference))
            {
                yield return new(result.Errors);

                yield break;
            }

            await foreach (var item in GetLeafToDoItemIdsAsync(context, reference, ignoreIds, ct))
            {
                yield return item;
            }

            yield break;
        }

        var entities = await context
            .Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == itemEntity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync(ct);

        if (entities.IsEmpty())
        {
            yield return itemEntity.Id.ToResult();

            yield break;
        }

        foreach (var entity in entities)
        {
            await foreach (var item in GetLeafToDoItemIdsAsync(context, entity, ignoreIds, ct))
            {
                yield return item;
            }
        }
    }

    private ConfiguredValueTaskAwaitable<Result> NormalizeOrderIndexAsync(
        SpravyDbToDoDbContext context,
        OptionStruct<Guid> parentId,
        CancellationToken ct
    )
    {
        var pi = parentId.TryGetValue(out var value) ? (Guid?)value : null;

        return context
            .Set<ToDoItemEntity>()
            .Where(x => x.ParentId == pi)
            .ToArrayEntitiesAsync(ct)
            .IfSuccessAsync(
                items =>
                {
                    var ordered = items.Span.OrderBy(x => x.OrderIndex);

                    for (var index = 0; index < ordered.Length; index++)
                    {
                        ordered[index].OrderIndex = (uint)index;
                    }

                    return Result.Success;
                },
                ct
            );
    }

    private async ValueTask<Result> GetParentsAsync(
        SpravyDbToDoDbContext context,
        Guid id,
        List<ToDoShortItem> parents,
        CancellationToken ct
    )
    {
        var parent = await context
            .Set<ToDoItemEntity>()
            .AsNoTracking()
            .Include(x => x.Parent)
            .SingleAsync(x => x.Id == id, ct);

        if (parent.Parent is null)
        {
            return Result.Success;
        }

        parents.Add(parent.Parent.ToToDoShortItem());

        return await GetParentsAsync(context, parent.Parent.Id, parents, ct);
    }

    private ConfiguredValueTaskAwaitable<Result> UpdateDueDateAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity item,
        TimeSpan offset,
        CancellationToken ct
    )
    {
        switch (item.Type)
        {
            case ToDoItemType.Value:
                return Result.AwaitableSuccess;
            case ToDoItemType.Group:
                return Result.AwaitableSuccess;
            case ToDoItemType.Planned:
                return Result.AwaitableSuccess;
            case ToDoItemType.Periodicity:
                return AddPeriodicity(item, ct).ToValueTaskResult().ConfigureAwait(false);
            case ToDoItemType.PeriodicityOffset:
                return AddPeriodicityOffset(item, offset, ct)
                    .ToValueTaskResult()
                    .ConfigureAwait(false);
            case ToDoItemType.Circle:
                return Result.AwaitableSuccess;
            case ToDoItemType.Step:
                return Result.AwaitableSuccess;
            case ToDoItemType.Reference:
                if (!item.ReferenceId.HasValue)
                {
                    return Result.AwaitableSuccess;
                }

                return context
                    .GetEntityAsync<ToDoItemEntity>(item.ReferenceId.Value)
                    .IfSuccessAsync(i => UpdateDueDateAsync(context, i, offset, ct), ct);
            default:
                return new Result(new ToDoItemTypeOutOfRangeError(item.Type))
                    .ToValueTaskResult()
                    .ConfigureAwait(false);
        }
    }

    private Result AddPeriodicityOffset(ToDoItemEntity item, TimeSpan offset, CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        if (item.IsRequiredCompleteInDueDate)
        {
            item.DueDate = item
                .DueDate.AddDays(item.DaysOffset + item.WeeksOffset * 7)
                .AddMonths(item.MonthsOffset)
                .AddYears(item.YearsOffset);
        }
        else
        {
            item.DueDate = DateTimeOffset
                .UtcNow.Add(offset)
                .Date.ToDateOnly()
                .AddDays(item.DaysOffset + item.WeeksOffset * 7)
                .AddMonths(item.MonthsOffset)
                .AddYears(item.YearsOffset);
        }

        return Result.Success;
    }

    private Result AddPeriodicity(ToDoItemEntity item, CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        switch (item.TypeOfPeriodicity)
        {
            case TypeOfPeriodicity.Daily:
                item.DueDate = item.DueDate.AddDays(1);

                break;
            case TypeOfPeriodicity.Weekly:
            {
                var dayOfWeek = item.DueDate.DayOfWeek;

                var daysOfWeek = item.GetDaysOfWeek()
                    .OrderByDefault(x => x)
                    .Select(x => (DayOfWeek?)x)
                    .ToArray();

                var nextDay = daysOfWeek.FirstOrDefault(x => x > dayOfWeek);

                item.DueDate = nextDay is not null
                    ? item.DueDate.AddDays((int)nextDay - (int)dayOfWeek)
                    : item.DueDate.AddDays(
                        7 - (int)dayOfWeek + (int)daysOfWeek.First().ThrowIfNullStruct()
                    );

                break;
            }
            case TypeOfPeriodicity.Monthly:
            {
                var now = item.DueDate;
                var dayOfMonth = now.Day;

                var daysOfMonth = item.GetDaysOfMonth()
                    .ToArray()
                    .Order()
                    .Select(x => (byte?)x)
                    .ThrowIfEmpty()
                    .ToArray();

                var nextDay = daysOfMonth.FirstOrDefault(x => x > dayOfMonth);
                var daysInCurrentMonth = DateTime.DaysInMonth(now.Year, now.Month);
                var daysInNextMonth = DateTime.DaysInMonth(
                    now.AddMonths(1).Year,
                    now.AddMonths(1).Month
                );

                item.DueDate = nextDay is not null
                    ? item.DueDate.WithDay(Math.Min(nextDay.Value, daysInCurrentMonth))
                    : item
                        .DueDate.AddMonths(1)
                        .WithDay(
                            Math.Min(daysOfMonth.First().ThrowIfNullStruct(), daysInNextMonth)
                        );

                break;
            }
            case TypeOfPeriodicity.Annually:
            {
                var now = item.DueDate;

                var daysOfYear = item.GetDaysOfYear()
                    .OrderBy(x => x)
                    .Select(x => (DayOfYear?)x)
                    .ToArray();

                var nextDay = daysOfYear.FirstOrDefault(x =>
                    x.ThrowIfNullStruct().Month >= now.Month && x.ThrowIfNullStruct().Day > now.Day
                );

                var daysInNextMonth = DateTime.DaysInMonth(
                    now.Year + 1,
                    daysOfYear.First().ThrowIfNullStruct().Month
                );

                item.DueDate = nextDay is not null
                    ? item
                        .DueDate.WithMonth(nextDay.Value.Month)
                        .WithDay(
                            Math.Min(
                                DateTime.DaysInMonth(now.Year, nextDay.Value.Month),
                                nextDay.Value.Day
                            )
                        )
                    : item
                        .DueDate.AddYears(1)
                        .WithMonth(daysOfYear.First().ThrowIfNullStruct().Month)
                        .WithDay(
                            Math.Min(daysInNextMonth, daysOfYear.First().ThrowIfNullStruct().Day)
                        );

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return Result.Success;
    }
}

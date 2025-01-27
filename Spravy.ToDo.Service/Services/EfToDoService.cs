using System.Collections.Frozen;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Spravy.Db.Models;
using Spravy.EventBus.Domain.Errors;
using Spravy.EventBus.Domain.Interfaces;

namespace Spravy.ToDo.Service.Services;

public class EfToDoService : IToDoService
{
    private static readonly ReadOnlyMemory<Guid> eventIds = new([AddToDoItemToFavoriteEventOptions.EventId,]);

    private readonly IFactory<ToDoSpravyDbContext> dbContextFactory;
    private readonly IEventBusService eventBusService;
    private readonly GetterToDoItemParametersService getterToDoItemParametersService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ISerializer serializer;

    public EfToDoService(
        IFactory<ToDoSpravyDbContext> dbContextFactory,
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

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> CloneToDoItemAsync(ReadOnlyMemory<Guid> cloneIds, OptionStruct<Guid> parentId, CancellationToken ct)
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () =>
                    {
                        var ids = cloneIds.ToArray();

                        return context.Set<ToDoItemEntity>().Where(x => ids.Contains(x.Id)).ToArrayEntitiesAsync(ct).IfSuccessForEachAsync(clone => AddCloneAsync(context, clone, parentId, ct).ConfigureAwait(false), ct);
                    },
                    ct
                ),
                ct
            );
    }

    public Cvtar ResetToDoItemAsync(ReadOnlyMemory<ResetToDoItemOptions> options, CancellationToken ct)
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();
        var dir = options.ToArray().ToDictionary(x => x.Id, x => x).ToFrozenDictionary();
        var keys = dir.Keys.ToArray().ToReadOnlyMemory();

        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () => GetAllChildrenAsync(context, keys, true, ct)
                       .IfSuccessAsync(
                            items => keys.IfSuccessForEachAsync(
                                id =>
                                {
                                    var item = items[id];

                                    return dir[item.Id]
                                       .ToResult()
                                       .IfSuccessAsync(
                                            o => Result.AwaitableSuccess
                                               .IfSuccessAsync(
                                                    () =>
                                                    {
                                                        if (o.IsCompleteCurrentTask)
                                                        {
                                                            item.IsCompleted = true;

                                                            return MoveNextDueDateAsync(context, item, offset, ct);
                                                        }

                                                        return Result.AwaitableSuccess;
                                                    },
                                                    ct
                                                )
                                               .IfSuccessAsync(
                                                    () => CircleCompletionAsync(
                                                            items,
                                                            item,
                                                            o.IsMoveCircleOrderIndex,
                                                            o.IsCompleteChildrenTask,
                                                            o.IsOnlyCompletedTasks,
                                                            ct
                                                        )
                                                       .IfSuccessAsync(() => StepCompletionAsync(items, item, o.IsCompleteChildrenTask, ct), ct),
                                                    ct
                                                ),
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

    public Cvtar EditToDoItemsAsync(EditToDoItems options, CancellationToken ct)
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () =>
                    {
                        var ids = options.Ids.ToArray();

                        return context.Set<ToDoItemEntity>()
                           .Where(x => ids.Contains(x.Id))
                           .Include(x => x.Reference)
                           .ToArrayEntitiesAsync(ct)
                           .IfSuccessForEachAsync(
                                item =>
                                {
                                    if (options.Name.IsEdit)
                                    {
                                        item.Name = options.Name.Value;
                                    }

                                    if (options.IsFavorite.IsEdit)
                                    {
                                        item.IsFavorite = options.IsFavorite.Value;
                                    }

                                    if (options.Type.IsEdit)
                                    {
                                        item.Type = options.Type.Value;
                                    }

                                    if (options.Description.IsEdit)
                                    {
                                        item.Description = options.Description.Value;
                                    }

                                    if (options.Link.IsEdit)
                                    {
                                        if (item.Type == ToDoItemType.Reference)
                                        {
                                            if (item.Reference is not null)
                                            {
                                                item.Reference.Link = options.Link.Value.TryGetValue(out var link) ? link.AbsoluteUri : string.Empty;
                                            }
                                        }
                                        else
                                        {
                                            item.Link = options.Link.Value.TryGetValue(out var link) ? link.AbsoluteUri : string.Empty;
                                        }
                                    }

                                    if (options.ParentId.IsEdit)
                                    {
                                        item.ParentId = options.ParentId.Value.GetValueOrNull();
                                    }

                                    if (options.DescriptionType.IsEdit)
                                    {
                                        item.DescriptionType = options.DescriptionType.Value;
                                    }

                                    if (options.ReferenceId.IsEdit)
                                    {
                                        item.ReferenceId = options.ReferenceId.Value.GetValueOrNull();
                                    }

                                    if (options.AnnuallyDays.IsEdit)
                                    {
                                        item.SetAnnuallyDays(options.AnnuallyDays.Value);
                                    }

                                    if (options.MonthlyDays.IsEdit)
                                    {
                                        item.SetMonthlyDays(options.MonthlyDays.Value);
                                    }

                                    if (options.ChildrenType.IsEdit)
                                    {
                                        item.ChildrenType = options.ChildrenType.Value;
                                    }

                                    if (options.DueDate.IsEdit)
                                    {
                                        item.DueDate = options.DueDate.Value;
                                    }

                                    if (options.DaysOffset.IsEdit)
                                    {
                                        item.DaysOffset = options.DaysOffset.Value;
                                    }

                                    if (options.MonthsOffset.IsEdit)
                                    {
                                        item.MonthsOffset = options.MonthsOffset.Value;
                                    }

                                    if (options.WeeksOffset.IsEdit)
                                    {
                                        item.WeeksOffset = options.WeeksOffset.Value;
                                    }

                                    if (options.YearsOffset.IsEdit)
                                    {
                                        item.YearsOffset = options.YearsOffset.Value;
                                    }

                                    if (options.IsRequiredCompleteInDueDate.IsEdit)
                                    {
                                        item.IsRequiredCompleteInDueDate = options.IsRequiredCompleteInDueDate.Value;
                                    }

                                    if (options.TypeOfPeriodicity.IsEdit)
                                    {
                                        item.TypeOfPeriodicity = options.TypeOfPeriodicity.Value;
                                    }

                                    if (options.WeeklyDays.IsEdit)
                                    {
                                        item.SetWeeklyDays(options.WeeklyDays.Value);
                                    }

                                    if (options.IsBookmark.IsEdit)
                                    {
                                        item.IsBookmark = options.IsBookmark.Value;
                                    }

                                    if (options.Icon.IsEdit)
                                    {
                                        item.Icon = options.Icon.Value;
                                    }

                                    if (options.Color.IsEdit)
                                    {
                                        item.Color = options.Color.Value;
                                    }

                                    if (options.RemindDaysBefore.IsEdit)
                                    {
                                        item.RemindDaysBefore = options.RemindDaysBefore.Value;
                                    }

                                    return Result.AwaitableSuccess;
                                },
                                ct
                            );
                    },
                    ct
                ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> UpdateEventsAsync(CancellationToken ct)
    {
        return eventBusService.GetEventsAsync(eventIds, ct)
           .IfSuccessAsync(
                events =>
                {
                    if (events.IsEmpty)
                    {
                        return false.ToResult().ToValueTaskResult().ConfigureAwait(false);
                    }

                    return dbContextFactory.Create()
                       .IfSuccessDisposeAsync(
                            context => context.AtomicExecuteAsync(
                                () => events.IfSuccessForEachAsync(
                                    e =>
                                    {
                                        if (e.Id == AddToDoItemToFavoriteEventOptions.EventId)
                                        {
                                            return serializer.DeserializeAsync<AddToDoItemToFavoriteEventOptions>(e.Content, ct)
                                               .IfSuccessAsync(
                                                    options => context.GetEntityAsync<ToDoItemEntity>(options.ToDoItemId)
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

                                        return new Result(new NotFoundEventError(e.Id)).ToValueTaskResult().ConfigureAwait(false);
                                    },
                                    ct
                                ),
                                ct
                            ),
                            ct
                        )
                       .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false), ct);
                },
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetBookmarkToDoItemIdsAsync(CancellationToken ct)
    {
        return dbContextFactory.Create().IfSuccessDisposeAsync(context => context.Set<ToDoItemEntity>().AsNoTracking().OrderBy(x => x.OrderIndex).ThenBy(x => x.NormalizeName).Where(x => x.IsBookmark).Select(x => x.Id).ToArrayEntitiesAsync(ct), ct);
    }

    public Cvtar RandomizeChildrenOrderIndexAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct)
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () => ids.IfSuccessForEachAsync(
                        id => context.Set<ToDoItemEntity>()
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
                ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> AddToDoItemAsync(ReadOnlyMemory<AddToDoItemOptions> o, CancellationToken ct)
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () => o.IfSuccessForEachAsync(
                        options =>
                        {
                            var parentId = options.ParentId.GetValueOrNull();
                            var id = Guid.NewGuid();

                            return context.Set<ToDoItemEntity>()
                               .AsNoTracking()
                               .Where(x => x.ParentId == parentId)
                               .Select(x => x.OrderIndex)
                               .ToArrayEntitiesAsync(ct)
                               .IfSuccessAsync(
                                    items =>
                                    {
                                        var toDoItem = options.ToToDoItemEntity();
                                        toDoItem.Id = id;
                                        toDoItem.OrderIndex = items.Length == 0 ? 0 : items.Max() + 1;

                                        return context.AddEntityAsync(toDoItem, ct).IfSuccessAsync(_ => id.ToResult(), ct);
                                    },
                                    ct
                                );
                        },
                        ct
                    ),
                    ct
                ),
                ct
            );
    }

    public Cvtar SwitchCompleteAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct)
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();
        var idsArray = ids.ToArray().ToReadOnlyMemory();

        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () => GetAllChildrenAsync(context, ids, true, ct)
                       .IfSuccessAsync(
                            items => idsArray.IfSuccessForEachAsync(
                                id =>
                                {
                                    var item = items[id];

                                    if (!item.IsCompleted)
                                    {
                                        return getterToDoItemParametersService.GetToDoItemParameters(items, new(), item, offset)
                                           .IfSuccessAsync(
                                                parameters =>
                                                {
                                                    if (!parameters.IsCan.HasFlag(ToDoItemIsCan.CanComplete))
                                                    {
                                                        return new Result(new ToDoItemAlreadyCompleteError(item.Id, item.Name)).ToValueTaskResult().ConfigureAwait(false);
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
                                                        case ToDoItemType.Reference:
                                                            break;
                                                        default:
                                                            return new Result(new ToDoItemTypeOutOfRangeError(item.Type)).ToValueTaskResult().ConfigureAwait(false);
                                                    }

                                                    return MoveNextDueDateAsync(context, item, offset, ct)
                                                       .IfSuccessAsync(
                                                            () =>
                                                            {
                                                                item.LastCompleted = DateTimeOffset.Now;

                                                                return CircleCompletionAsync(
                                                                        items,
                                                                        item,
                                                                        true,
                                                                        false,
                                                                        false,
                                                                        ct
                                                                    )
                                                                   .IfSuccessAsync(() => StepCompletionAsync(items, item, false, ct), ct);
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
                ),
                ct
            );
    }

    public Cvtar UpdateToDoItemOrderIndexAsync(ReadOnlyMemory<UpdateOrderIndexToDoItemOptions> o, CancellationToken ct)
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () => o.IfSuccessForEachAsync(
                        options => context.GetEntityAsync<ToDoItemEntity>(options.Id)
                           .IfSuccessAsync(
                                item => context.GetEntityAsync<ToDoItemEntity>(options.TargetId)
                                   .IfSuccessAsync(
                                        targetItem =>
                                        {
                                            var orderIndex = options.IsAfter ? targetItem.OrderIndex + 1 : targetItem.OrderIndex;

                                            return context.Set<ToDoItemEntity>()
                                               .Where(x => x.ParentId == item.ParentId && x.Id != item.Id && x.OrderIndex >= orderIndex)
                                               .ToArrayEntitiesAsync(ct)
                                               .IfSuccessAsync(
                                                    items =>
                                                    {
                                                        foreach (var itemEntity in items.Span)
                                                        {
                                                            itemEntity.OrderIndex++;
                                                        }

                                                        item.OrderIndex = orderIndex;

                                                        return NormalizeOrderIndexAsync(context, item.ParentId.ToOption(), ct);
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
                ),
                ct
            );
    }

    public Cvtar DeleteToDoItemsAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct)
    {
        var idsArray = ids.Span.ToArray();

        return dbContextFactory.Create().IfSuccessDisposeAsync(context => context.AtomicExecuteAsync(() => context.Set<ToDoItemEntity>().Where(x => idsArray.Contains(x.Id)).ToArrayEntitiesAsync(ct).IfSuccessForEachAsync(item => DeleteToDoItemAsync(item.Id, context, ct).IfSuccessAsync(() => NormalizeOrderIndexAsync(context, item.ParentId.ToOption(), ct), ct), ct), ct), ct);
    }

    public ConfiguredValueTaskAwaitable<Result<ToDoResponse>> GetAsync(GetToDo get, CancellationToken ct)
    {
        var offset = httpContextAccessor.HttpContext.ThrowIfNull().GetTimeZoneOffset();

        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.Set<ToDoItemEntity>()
                   .AsNoTracking()
                   .ToArrayEntitiesAsync(ct)
                   .IfSuccessAsync(
                        items =>
                        {
                            var dictionary = items.ToDictionary(x => x.Id).ToFrozenDictionary();
                            var fullDictionary = new Dictionary<Guid, FullToDoItem>();
                            var selectorItems = new ToDoSelectorItemsResponse(false, ReadOnlyMemory<ToDoSelectorItem>.Empty);
                            var toStringItems = ReadOnlyMemory<ToStringItem>.Empty;
                            var roots = dictionary.Values.Where(x => x.ParentId is null).ToArray().ToReadOnlyMemory();
                            var currentActive = OptionStruct<ToDoShortItem>.Default;
                            var activeItems = ReadOnlyMemory<ActiveItem>.Empty;
                            var favoriteItems = new ToDoFullItemsResponse(false, ReadOnlyMemory<FullToDoItem>.Empty);
                            var bookmarkItems = new ToDoShortItemsResponse(false, ReadOnlyMemory<ToDoShortItem>.Empty);
                            var childrenItems = ReadOnlyMemory<ChildrenItem>.Empty;
                            var leafItems = ReadOnlyMemory<LeafItem>.Empty;
                            var searchItems = new ToDoFullItemsResponse(false, ReadOnlyMemory<FullToDoItem>.Empty);
                            var parentItems = ReadOnlyMemory<ParentItem>.Empty;
                            var todayItems = new ToDoFullItemsResponse(false, ReadOnlyMemory<FullToDoItem>.Empty);
                            var rootItems = new ToDoFullItemsResponse(false, ReadOnlyMemory<FullToDoItem>.Empty);
                            var fullItems = new ToDoFullItemsResponse(false, ReadOnlyMemory<FullToDoItem>.Empty);

                            if (get.IsSelectorItems)
                            {
                                selectorItems = new(true, roots.IfSuccessForEach(x => GetToDoSelectorItems(items, x.Id).IfSuccess(children => new ToDoSelectorItem(x.ToToDoShortItem(), children).ToResult())).ThrowIfError());
                            }

                            if (!get.ToStringItems.IsEmpty)
                            {
                                toStringItems = get.ToStringItems
                                   .IfSuccessForEach(
                                        item => item.Ids.IfSuccessForEach(
                                            x =>
                                            {
                                                var builder = new StringBuilder();

                                                return ToDoItemToString(
                                                        dictionary,
                                                        fullDictionary,
                                                        new(item.Statuses, x),
                                                        0,
                                                        builder,
                                                        offset
                                                    )
                                                   .IfSuccess(() => new ToStringItem(x, builder.ToString().Trim()).ToResult());
                                            },
                                            ct
                                        )
                                    )
                                   .ThrowIfError()
                                   .SelectMany();
                            }

                            if (get.IsCurrentActiveItem)
                            {
                                var rootsFullItems = roots.IfSuccessForEach(i => GetFullItem(dictionary, fullDictionary, i, offset)).ThrowIfError().OrderBy(x => x.Item.OrderIndex);

                                foreach (var rootsFullItem in rootsFullItems.Span)
                                {
                                    if (rootsFullItem.Status == ToDoItemStatus.Miss)
                                    {
                                        currentActive = rootsFullItem.Active;

                                        break;
                                    }

                                    switch (rootsFullItem.Status)
                                    {
                                        case ToDoItemStatus.ReadyForComplete:
                                            if (!currentActive.IsHasValue)
                                            {
                                                currentActive = rootsFullItem.Active;
                                            }

                                            break;
                                        case ToDoItemStatus.Planned:
                                            break;
                                        case ToDoItemStatus.Completed:
                                            break;
                                        case ToDoItemStatus.ComingSoon:
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                }
                            }

                            if (!get.ActiveItems.IsEmpty)
                            {
                                activeItems = get.ActiveItems.IfSuccessForEach(x => GetFullItem(dictionary, fullDictionary, dictionary[x], offset)).IfSuccessForEach(x => new ActiveItem(x.Item.Id, x.Active).ToResult()).ThrowIfError();
                            }

                            if (get.IsFavoriteItems)
                            {
                                favoriteItems = new(true, dictionary.Where(x => x.Value.IsFavorite).ToArray().ToReadOnlyMemory().IfSuccessForEach(x => GetFullItem(dictionary, fullDictionary, x.Value, offset)).ThrowIfError());
                            }

                            if (get.IsBookmarkItems)
                            {
                                bookmarkItems = new(true, dictionary.Where(x => x.Value.IsBookmark).Select(x => x.Value.ToToDoShortItem()).ToArray().ToReadOnlyMemory());
                            }

                            if (!get.ChildrenItems.IsEmpty)
                            {
                                childrenItems = get.ChildrenItems.IfSuccessForEach(id => new ChildrenItem(id, dictionary.Values.Where(x => x.ParentId == id).ToArray().ToReadOnlyMemory().IfSuccessForEach(item => GetFullItem(dictionary, fullDictionary, item, offset)).ThrowIfError()).ToResult()).ThrowIfError();
                            }

                            if (!get.LeafItems.IsEmpty)
                            {
                                leafItems = get.LeafItems
                                   .IfSuccessForEach(
                                        id => new LeafItem(
                                            id,
                                            GetLeafToDoItems(
                                                    dictionary,
                                                    fullDictionary,
                                                    dictionary[id],
                                                    new(),
                                                    offset
                                                )
                                               .ToArray()
                                        ).ToResult()
                                    )
                                   .ThrowIfError();
                            }

                            if (!get.SearchText.IsNullOrWhiteSpace())
                            {
                                searchItems = new(true, dictionary.Values.Where(x => x.Name.Contains(get.SearchText, StringComparison.InvariantCultureIgnoreCase)).ToArray().ToReadOnlyMemory().IfSuccessForEach(x => GetFullItem(dictionary, fullDictionary, x, offset)).ThrowIfError());
                            }

                            if (!get.ParentItems.IsEmpty)
                            {
                                parentItems = get.ParentItems.IfSuccessForEach(x => new ParentItem(x, GetParents(dictionary, x).Reverse().ToArray()).ToResult()).ThrowIfError();
                            }

                            if (get.IsTodayItems)
                            {
                                var today = DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly();
                                todayItems = new(true, dictionary.Values.Where(x => !x.IsCompleted && (x.Type == ToDoItemType.Periodicity || x.Type == ToDoItemType.PeriodicityOffset || x.Type == ToDoItemType.Planned)).Where(x => x.DueDate <= today || x.RemindDaysBefore != 0 && today >= x.DueDate.AddDays((int)-x.RemindDaysBefore)).ToArray().ToReadOnlyMemory().IfSuccessForEach(x => GetFullItem(dictionary, fullDictionary, x, offset)).ThrowIfError());
                            }

                            if (get.IsRootItems)
                            {
                                rootItems = new(true, roots.IfSuccessForEach(x => GetFullItem(dictionary, fullDictionary, x, offset)).ThrowIfError());
                            }

                            if (!get.Items.IsEmpty)
                            {
                                fullItems = new(true, get.Items.IfSuccessForEach(x => GetFullItem(dictionary, fullDictionary, dictionary[x], offset)).ThrowIfError());
                            }

                            return new ToDoResponse(
                                selectorItems,
                                toStringItems,
                                currentActive,
                                activeItems,
                                favoriteItems,
                                bookmarkItems,
                                childrenItems,
                                leafItems,
                                searchItems,
                                parentItems,
                                todayItems,
                                rootItems,
                                fullItems
                            ).ToResult();
                        },
                        ct
                    ),
                ct
            );
    }

    private void SetRandomOrderIndex(ReadOnlyMemory<ToDoItemEntity> entities)
    {
        foreach (var entity in entities.Span)
        {
            entity.OrderIndex = (uint)RandomNumberGenerator.GetInt32(0, int.MaxValue);
        }
    }

    private async ValueTask<Result<Guid>> AddCloneAsync(ToDoSpravyDbContext context, ToDoItemEntity clone, OptionStruct<Guid> parentId, CancellationToken ct)
    {
        var id = clone.Id;
        clone.Id = Guid.NewGuid();
        clone.ParentId = parentId.TryGetValue(out var value) ? value : null;
        await context.AddAsync(clone, ct);
        var items = await context.Set<ToDoItemEntity>().AsNoTracking().Where(x => x.ParentId == id).ToArrayAsync(ct);

        foreach (var item in items)
        {
            await AddCloneAsync(context, item, clone.Id.ToOption(), ct);
        }

        return clone.Id.ToResult();
    }

    private Cvtar DeleteToDoItemAsync(Guid id, ToDoSpravyDbContext context, CancellationToken ct)
    {
        return context.GetEntityAsync<ToDoItemEntity>(id)
           .IfSuccessAsync(
                item => context.Set<ToDoItemEntity>()
                   .AsNoTracking()
                   .Where(x => x.ParentId == id)
                   .ToArrayEntitiesAsync(ct)
                   .IfSuccessForEachAsync(child => DeleteToDoItemAsync(child.Id, context, ct), ct)
                   .IfSuccessAsync(
                        () => context.Set<ToDoItemEntity>()
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

    private Cvtar StepCompletionAsync(FrozenDictionary<Guid, ToDoItemEntity> items, ToDoItemEntity item, bool completeTask, CancellationToken ct)
    {
        return items.Where(x => x.Value.ParentId == item.Id && x.Value.Type == ToDoItemType.Step)
           .Select(x => x.Value)
           .ToArray()
           .ToReadOnlyMemory()
           .ToResult()
           .IfSuccessAsync(
                steps =>
                {
                    foreach (var step in steps.Span)
                    {
                        step.IsCompleted = completeTask;
                    }

                    return items.Where(x => x.Value.ParentId == item.Id && x.Value.Type == ToDoItemType.Group).Select(x => x.Value).ToArray().ToReadOnlyMemory().IfSuccessForEachAsync(group => StepCompletionAsync(items, group, completeTask, ct), ct);
                },
                ct
            )
           .IfSuccessAsync(
                () => items.Where(x => x.Value.ParentId == item.Id && x.Value.Type == ToDoItemType.Reference && x.Value.ReferenceId.HasValue)
                   .Select(x => x.Value.ReferenceId.ThrowIfNullStruct())
                   .ToArray()
                   .ToReadOnlyMemory()
                   .IfSuccessForEachAsync(
                        referenceId =>
                        {
                            var reference = items[referenceId];

                            return reference.Type switch
                            {
                                ToDoItemType.Value => Result.AwaitableSuccess,
                                ToDoItemType.Group => StepCompletionAsync(items, reference, completeTask, ct),
                                ToDoItemType.Planned => Result.AwaitableSuccess,
                                ToDoItemType.Periodicity => Result.AwaitableSuccess,
                                ToDoItemType.PeriodicityOffset => Result.AwaitableSuccess,
                                ToDoItemType.Circle => Result.AwaitableSuccess,
                                ToDoItemType.Step => Result.Execute(() => reference.IsCompleted = completeTask).ToValueTaskResult().ConfigureAwait(false),
                                ToDoItemType.Reference => Result.AwaitableSuccess,
                                _ => new Result(new ToDoItemTypeOutOfRangeError(reference.Type)).ToValueTaskResult().ConfigureAwait(false),
                            };
                        },
                        ct
                    ),
                ct
            );
    }

    private Cvtar CircleCompletionAsync(
        FrozenDictionary<Guid, ToDoItemEntity> items,
        ToDoItemEntity item,
        bool moveCircleOrderIndex,
        bool completeTask,
        bool onlyCompletedTasks,
        CancellationToken ct
    )
    {
        return items.Where(x => x.Value.ParentId == item.Id && x.Value.Type == ToDoItemType.Circle)
           .Select(x => x.Value)
           .OrderBy(x => x.OrderIndex)
           .ToArray()
           .ToReadOnlyMemory()
           .ToResult()
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
                                var next = circleChildren.Span.FirstOrDefault(x => x.OrderIndex > item.CurrentCircleOrderIndex);
                                nextOrderIndex = next?.OrderIndex ?? circleChildren.Span[0].OrderIndex;
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

                    return items.Where(x => x.Value.ParentId == item.Id && x.Value.Type == ToDoItemType.Group)
                   .Select(x => x.Value)
                   .ToArray()
                   .ToReadOnlyMemory()
                   .IfSuccessForEachAsync(
                        group => CircleCompletionAsync(
                            items,
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
                () => items.Where(x => x.Value.ParentId == item.Id && x.Value.Type == ToDoItemType.Reference && x.Value.ReferenceId.HasValue)
                   .Select(x => x.Value.ReferenceId.ThrowIfNullStruct())
                   .ToArray()
                   .ToReadOnlyMemory()
                   .IfSuccessForEachAsync(
                        referenceId =>
                        {
                            var reference = items[referenceId];

                            return reference.Type switch
                            {
                                ToDoItemType.Value => Result.AwaitableSuccess,
                                ToDoItemType.Group => CircleCompletionAsync(
                                    items,
                                    reference,
                                    moveCircleOrderIndex,
                                    completeTask,
                                    onlyCompletedTasks,
                                    ct
                                ),
                                ToDoItemType.Planned => Result.AwaitableSuccess,
                                ToDoItemType.Periodicity => Result.AwaitableSuccess,
                                ToDoItemType.PeriodicityOffset => Result.AwaitableSuccess,
                                ToDoItemType.Circle => Result.AwaitableSuccess,
                                ToDoItemType.Step => Result.AwaitableSuccess,
                                ToDoItemType.Reference => Result.AwaitableSuccess,
                                _ => new Result(new ToDoItemTypeOutOfRangeError(reference.Type)).ToValueTaskResult().ConfigureAwait(false),
                            };
                        },
                        ct
                    ),
                ct
            );
    }

    private Result ToDoItemToString(
        FrozenDictionary<Guid, ToDoItemEntity> items,
        Dictionary<Guid, FullToDoItem> fullToDoItems,
        ToDoItemToStringOptions options,
        ushort level,
        StringBuilder builder,
        TimeSpan offset
    )
    {
        return items.Values
           .Where(x => x.ParentId == options.Id)
           .OrderBy(x => x.OrderIndex)
           .ToArray()
           .ToReadOnlyMemory()
           .ToResult()
           .IfSuccessForEach(
                item => GetFullItem(items, fullToDoItems, item, offset)
                   .IfSuccess(
                        parameters =>
                        {
                            if (!options.Statuses.Select(x => (byte)x).Span.Contains((byte)parameters.Status))
                            {
                                return Result.Success;
                            }

                            builder.Duplicate(" ", level);
                            builder.Append(item.Name);
                            builder.AppendLine();

                            return ToDoItemToString(
                                items,
                                fullToDoItems,
                                new(options.Statuses, item.Id),
                                (ushort)(level + 1),
                                builder,
                                offset
                            );
                        }
                    )
            );
    }

    private Result<FullToDoItem> GetFullItem(FrozenDictionary<Guid, ToDoItemEntity> allItems, Dictionary<Guid, FullToDoItem> fullToDoItems, ToDoItemEntity entity, TimeSpan offset)
    {
        if (fullToDoItems.TryGetValue(entity.Id, out var value))
        {
            return value.ToResult();
        }

        return getterToDoItemParametersService.GetToDoItemParameters(allItems, fullToDoItems, entity, offset).IfSuccess(p => entity.ToFullToDoItem(p).ToResult());
    }

    private Result<ReadOnlyMemory<ToDoSelectorItem>> GetToDoSelectorItems(ReadOnlyMemory<ToDoItemEntity> items, Guid id)
    {
        return items.Where(x => x.ParentId == id).OrderBy(x => x.OrderIndex).IfSuccessForEach(item => GetToDoSelectorItems(items, item.Id).IfSuccess(children => new ToDoSelectorItem(item.ToToDoShortItem(), children).ToResult()));
    }

    private IEnumerable<FullToDoItem> GetLeafToDoItems(
        FrozenDictionary<Guid, ToDoItemEntity> allItems,
        Dictionary<Guid, FullToDoItem> fullToDoItems,
        ToDoItemEntity entity,
        List<Guid> ignoreIds,
        TimeSpan offset
    )
    {
        if (ignoreIds.Contains(entity.Id))
        {
            yield break;
        }

        if (entity.Type == ToDoItemType.Reference)
        {
            ignoreIds.Add(entity.Id);

            if (entity.ReferenceId is null)
            {
                yield return GetFullItem(allItems, fullToDoItems, entity, offset).ThrowIfError();

                yield break;
            }

            var reference = allItems[entity.ReferenceId.Value];

            foreach (var item in GetLeafToDoItems(
                allItems,
                fullToDoItems,
                reference,
                ignoreIds,
                offset
            ))
            {
                yield return item;
            }

            yield break;
        }

        var entities = allItems.Values.Where(x => x.ParentId == entity.Id).OrderBy(x => x.OrderIndex).ToArray();

        if (entities.IsEmpty())
        {
            yield return GetFullItem(allItems, fullToDoItems, entity, offset).ThrowIfError();

            yield break;
        }

        foreach (var e in entities)
        {
            foreach (var item in GetLeafToDoItems(
                allItems,
                fullToDoItems,
                e,
                ignoreIds,
                offset
            ))
            {
                yield return item;
            }
        }
    }

    private Cvtar NormalizeOrderIndexAsync(ToDoSpravyDbContext context, OptionStruct<Guid> parentId, CancellationToken ct)
    {
        var pi = parentId.TryGetValue(out var value) ? (Guid?)value : null;

        return context.Set<ToDoItemEntity>()
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

    private IEnumerable<ToDoShortItem> GetParents(FrozenDictionary<Guid, ToDoItemEntity> allItems, Guid id)
    {
        var parent = allItems[id];

        yield return parent.ToToDoShortItem();

        if (parent.ParentId is null)
        {
            yield break;
        }

        foreach (var item in GetParents(allItems, parent.ParentId.Value))
        {
            yield return item;
        }
    }

    private Cvtar MoveNextDueDateAsync(ToDoSpravyDbContext context, ToDoItemEntity item, TimeSpan offset, CancellationToken ct)
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
                return AddPeriodicity(item, offset, ct).ToValueTaskResult().ConfigureAwait(false);
            case ToDoItemType.PeriodicityOffset:
                return AddPeriodicityOffset(item, offset, ct).ToValueTaskResult().ConfigureAwait(false);
            case ToDoItemType.Circle:
                return Result.AwaitableSuccess;
            case ToDoItemType.Step:
                return Result.AwaitableSuccess;
            case ToDoItemType.Reference:
                if (!item.ReferenceId.HasValue)
                {
                    return Result.AwaitableSuccess;
                }

                return context.GetEntityAsync<ToDoItemEntity>(item.ReferenceId.Value).IfSuccessAsync(i => MoveNextDueDateAsync(context, i, offset, ct), ct);
            default:
                return new Result(new ToDoItemTypeOutOfRangeError(item.Type)).ToValueTaskResult().ConfigureAwait(false);
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
            item.DueDate = item.DueDate.AddDays(item.DaysOffset + item.WeeksOffset * 7).AddMonths(item.MonthsOffset).AddYears(item.YearsOffset);
        }
        else
        {
            item.DueDate = DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly().AddDays(item.DaysOffset + item.WeeksOffset * 7).AddMonths(item.MonthsOffset).AddYears(item.YearsOffset);
        }

        return Result.Success;
    }

    private Result AddPeriodicity(ToDoItemEntity item, TimeSpan offset, CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        var currentDueDate = item.IsRequiredCompleteInDueDate ? item.DueDate : DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly();

        switch (item.TypeOfPeriodicity)
        {
            case TypeOfPeriodicity.Daily:
                item.DueDate = currentDueDate.AddDays(1);

                break;
            case TypeOfPeriodicity.Weekly:
            {
                var dayOfWeek = currentDueDate.DayOfWeek;
                var daysOfWeek = item.GetDaysOfWeek().OrderByDefault(x => x).Select(x => (DayOfWeek?)x).ToArray();
                var nextDay = daysOfWeek.FirstOrDefault(x => x > dayOfWeek);
                item.DueDate = nextDay is not null ? currentDueDate.AddDays((int)nextDay - (int)dayOfWeek) : currentDueDate.AddDays(7 - (int)dayOfWeek + (int)daysOfWeek.First().ThrowIfNullStruct());

                break;
            }
            case TypeOfPeriodicity.Monthly:
            {
                var dayOfMonth = currentDueDate.Day;
                var daysOfMonth = item.GetDaysOfMonth().ToArray().Order().Select(x => (byte?)x).ThrowIfEmpty().ToArray();
                var nextDay = daysOfMonth.FirstOrDefault(x => x > dayOfMonth);
                var daysInCurrentMonth = DateTime.DaysInMonth(currentDueDate.Year, currentDueDate.Month);
                var daysInNextMonth = DateTime.DaysInMonth(currentDueDate.AddMonths(1).Year, currentDueDate.AddMonths(1).Month);
                item.DueDate = nextDay is not null ? item.DueDate.WithDay(Math.Min(nextDay.Value, daysInCurrentMonth)) : item.DueDate.AddMonths(1).WithDay(Math.Min(daysOfMonth.First().ThrowIfNullStruct(), daysInNextMonth));

                break;
            }
            case TypeOfPeriodicity.Annually:
            {
                var daysOfYear = item.GetDaysOfYear().OrderBy(x => x).Select(x => (DayOfYear?)x).ToArray();
                var nextDay = daysOfYear.FirstOrDefault(x => x.ThrowIfNullStruct().Month >= (Month)currentDueDate.Month && x.ThrowIfNullStruct().Day > currentDueDate.Day);
                var daysInNextMonth = DateTime.DaysInMonth(currentDueDate.Year + 1, (byte)daysOfYear.First().ThrowIfNullStruct().Month);
                item.DueDate = nextDay is not null ? item.DueDate.WithMonth((byte)nextDay.Value.Month).WithDay(Math.Min(DateTime.DaysInMonth(currentDueDate.Year, (byte)nextDay.Value.Month), nextDay.Value.Day)) : item.DueDate.AddYears(1).WithMonth((byte)daysOfYear.First().ThrowIfNullStruct().Month).WithDay(Math.Min(daysInNextMonth, daysOfYear.First().ThrowIfNullStruct().Day));

                break;
            }
            default:
                return new(new TypeOfPeriodicityOutOfRangeError(item.TypeOfPeriodicity));
        }

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result<FrozenDictionary<Guid, ToDoItemEntity>>> GetAllChildrenAsync(ToDoSpravyDbContext context, ReadOnlyMemory<Guid> ids, bool tracking, CancellationToken ct)
    {
        var parameters = CreateSqlRawParametersForAllChildren(ids);
        var query = context.Set<ToDoItemEntity>().FromSqlRaw(parameters.Sql, parameters.Parameters.ToArray());

        if (tracking)
        {
            query.AsTracking();
        }
        else
        {
            query.AsNoTracking();
        }

        return query.ToArrayEntitiesAsync(ct)
           .IfSuccessAsync(
                items =>
                {
                    var dictionary = new Dictionary<Guid, ToDoItemEntity>();

                    foreach (var item in items.Span)
                    {
                        dictionary.TryAdd(item.Id, item);
                    }

                    return dictionary.ToFrozenDictionary().ToResult();
                },
                ct
            );
    }

    private SqlRawParameters CreateSqlRawParametersForAllChildren(ReadOnlyMemory<Guid> ids)
    {
        var idsString = Enumerable.Range(0, ids.Length).Select(i => $"@Id{i}").JoinString(", ");
        var parameters = new DbParameter[ids.Length];

        for (var i = 0; i < ids.Length; i++)
        {
            parameters[i] = new SqliteParameter($"@Id{i}", ids.Span[i]);
        }

        return new(
            $"""
            WITH RECURSIVE hierarchy(
                     Id,
                     Name,
                     OrderIndex,
                     Description,
                     CreatedDateTime,
                     Type,
                     IsFavorite,
                     DueDate,
                     IsCompleted,
                     TypeOfPeriodicity,
                     DaysOfWeek,
                     DaysOfMonth,
                     DaysOfYear,
                     LastCompleted,
                     DaysOffset,
                     MonthsOffset,
                     WeeksOffset,
                     YearsOffset,
                     ChildrenType,
                     CurrentCircleOrderIndex,
                     Link,
                     IsRequiredCompleteInDueDate,
                     DescriptionType,
                     ReferenceId,
                     ParentId,
                     IsBookmark,
                     Icon,
                     Color,
                     RemindDaysBefore,
                     NormalizeName
                 ) AS (
                     SELECT
                     Id,
                     Name,
                     OrderIndex,
                     Description,
                     CreatedDateTime,
                     Type,
                     IsFavorite,
                     DueDate,
                     IsCompleted,
                     TypeOfPeriodicity,
                     DaysOfWeek,
                     DaysOfMonth,
                     DaysOfYear,
                     LastCompleted,
                     DaysOffset,
                     MonthsOffset,
                     WeeksOffset,
                     YearsOffset,
                     ChildrenType,
                     CurrentCircleOrderIndex,
                     Link,
                     IsRequiredCompleteInDueDate,
                     DescriptionType,
                     ReferenceId,
                     ParentId,
                     IsBookmark,
                     Icon,
                     Color,
                     RemindDaysBefore,
                     NormalizeName
                     FROM ToDoItem
                     WHERE Id IN ({idsString})
            
                     UNION ALL
            
                     SELECT
                     t.Id,
                     t.Name,
                     t.OrderIndex,
                     t.Description,
                     t.CreatedDateTime,
                     t.Type,
                     t.IsFavorite,
                     t.DueDate,
                     t.IsCompleted,
                     t.TypeOfPeriodicity,
                     t.DaysOfWeek,
                     t.DaysOfMonth,
                     t.DaysOfYear,
                     t.LastCompleted,
                     t.DaysOffset,
                     t.MonthsOffset,
                     t.WeeksOffset,
                     t.YearsOffset,
                     t.ChildrenType,
                     t.CurrentCircleOrderIndex,
                     t.Link,
                     t.IsRequiredCompleteInDueDate,
                     t.DescriptionType,
                     t.ReferenceId,
                     t.ParentId,
                     t.IsBookmark,
                     t.Icon,
                     t.Color,
                     t.RemindDaysBefore,
                     t.NormalizeName
                     FROM ToDoItem t
                     INNER JOIN hierarchy h ON t.ParentId = h.Id
            
                     UNION ALL
            
                     SELECT
                     t.Id,
                     t.Name,
                     t.OrderIndex,
                     t.Description,
                     t.CreatedDateTime,
                     t.Type,
                     t.IsFavorite,
                     t.DueDate,
                     t.IsCompleted,
                     t.TypeOfPeriodicity,
                     t.DaysOfWeek,
                     t.DaysOfMonth,
                     t.DaysOfYear,
                     t.LastCompleted,
                     t.DaysOffset,
                     t.MonthsOffset,
                     t.WeeksOffset,
                     t.YearsOffset,
                     t.ChildrenType,
                     t.CurrentCircleOrderIndex,
                     t.Link,
                     t.IsRequiredCompleteInDueDate,
                     t.DescriptionType,
                     t.ReferenceId,
                     t.ParentId,
                     t.IsBookmark,
                     t.Icon,
                     t.Color,
                     t.RemindDaysBefore,
                     t.NormalizeName
                     FROM ToDoItem t
                     INNER JOIN hierarchy h ON t.Id = h.ReferenceId
                     WHERE h.Type = 7 AND h.ReferenceId IS NOT NULL AND h.ReferenceId <> h.Id
                 )
                 SELECT * FROM hierarchy;
            """,
            parameters
        );
    }
}
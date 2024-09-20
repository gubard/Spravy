namespace Spravy.Schedule.Service.Services;

public class EfScheduleService : IScheduleService
{
    private readonly IFactory<SpravyDbScheduleDbContext> dbContextFactory;
    private readonly IEventBusService eventBusService;

    public EfScheduleService(
        IFactory<SpravyDbScheduleDbContext> dbContextFactory,
        IEventBusService eventBusService
    )
    {
        this.dbContextFactory = dbContextFactory;
        this.eventBusService = eventBusService;
    }

    public Cvtar AddTimerAsync(AddTimerParameters parameters, CancellationToken ct)
    {
        var newTimer = new TimerEntity
        {
            Id = Guid.NewGuid(),
            EventId = parameters.EventId,
            DueDateTime = parameters.DueDateTime,
            Content = parameters.Content.ToArray(),
            Name = parameters.Name,
        };

        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .Set<TimerEntity>()
                                .AddEntityAsync(newTimer, ct)
                                .ToResultOnlyAsync(),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> UpdateEventsAsync(CancellationToken ct)
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .Set<TimerEntity>()
                                .Where(x => x.DueDateTime < DateTime.Now)
                                .AsNoTracking()
                                .ToArrayEntitiesAsync(ct)
                                .IfSuccessAsync(
                                    items =>
                                    {
                                        if (items.IsEmpty)
                                        {
                                            return false
                                                .ToResult()
                                                .ToValueTaskResult()
                                                .ConfigureAwait(false);
                                        }

                                        return items
                                            .IfSuccessForEachAsync(
                                                item =>
                                                    eventBusService.PublishEventAsync(
                                                        item.EventId,
                                                        item.Content,
                                                        ct
                                                    ),
                                                ct
                                            )
                                            .IfSuccessAsync(
                                                () => context.RemoveRangeEntities(items),
                                                ct
                                            )
                                            .IfSuccessAsync(() => true.ToResult(), ct);
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TimerItem>>> GetTimersAsync(
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
                                .Set<TimerEntity>()
                                .AsNoTracking()
                                .ToArrayEntitiesAsync(ct)
                                .IfSuccessAsync(timers => timers.ToTimerItem().ToResult(), ct),
                        ct
                    ),
                ct
            );
    }

    public Cvtar RemoveTimerAsync(Guid id, CancellationToken ct)
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<TimerEntity>(id)
                                .IfSuccessAsync(context.RemoveEntity, ct),
                        ct
                    ),
                ct
            );
    }
}

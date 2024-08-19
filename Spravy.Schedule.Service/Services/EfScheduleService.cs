namespace Spravy.Schedule.Service.Services;

public class EfScheduleService : IScheduleService
{
    private readonly IFactory<SpravyDbScheduleDbContext> dbContextFactory;

    public EfScheduleService(IFactory<SpravyDbScheduleDbContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public ConfiguredValueTaskAwaitable<Result> AddTimerAsync(
        AddTimerParameters parameters,
        CancellationToken ct
    )
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

    public ConfiguredValueTaskAwaitable<Result> RemoveTimerAsync(Guid id, CancellationToken ct)
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

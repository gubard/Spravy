using Microsoft.EntityFrameworkCore;
using Spravy.Db.Extensions;
using Spravy.EventBus.Db.Models;

namespace Spravy.EventBus.Service.Services;

public class EventStorage
{
    private static readonly TimeSpan EventLiveTime = TimeSpan.FromMinutes(1);
    private static readonly Dictionary<string, DateTime> lastWriteTimeUtc = new();
    private readonly IFactory<SpravyDbEventBusDbContext> dbContextFactory;
    private readonly IFactory<FileInfo> fileFactory;

    public EventStorage(
        IFactory<SpravyDbEventBusDbContext> dbContextFactory,
        IFactory<FileInfo> fileFactory
    )
    {
        this.dbContextFactory = dbContextFactory;
        this.fileFactory = fileFactory;
    }

    public ConfiguredValueTaskAwaitable<Result> AddEventAsync(
        Guid id,
        byte[] content,
        CancellationToken ct
    )
    {
        var newEvent = new EventEntity
        {
            EventId = id,
            Content = content,
            Id = Guid.NewGuid(),
        };

        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .Set<EventEntity>()
                                .AddEntityAsync(newEvent, ct)
                                .ToResultOnlyAsync(),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<EventValue>>> PushEventAsync(
        ReadOnlyMemory<Guid> eventIds,
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
                                .Set<EventEntity>()
                                .AsNoTracking()
                                .Where(x => eventIds.ToArray().Contains(x.EventId))
                                .ToArrayEntitiesAsync(ct)
                                .IfSuccessAsync(
                                    y =>
                                        y.IfSuccessForEach(x =>
                                                new EventValue(x.EventId, x.Content).ToResult()
                                            )
                                            .IfSuccess(r =>
                                                context
                                                    .RemoveRangeEntities(y)
                                                    .IfSuccess(() => r.ToResult())
                                            ),
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }
}

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

    public Task<ReadOnlyMemory<EventValue>> PushEventAsync(ReadOnlyMemory<Guid> eventIds)
    {
        return ReadOnlyMemory<EventValue>.Empty.ToTaskResult();
        /*var file = fileFactory.Create();

        if (!lastWriteTimeUtc.TryGetValue(file.FullName, out var date))
        {
            lastWriteTimeUtc[file.FullName] = file.LastWriteTimeUtc;
        }
        else if (file.LastWriteTimeUtc == date)
        {
            return Enumerable.Empty<EventValue>();
        }

        await using var context = dbContextFactory.Create();

        var result = await context.AtomicExecuteAsync(
            async c =>
            {
                var removed = await c.Set<EventEntity>()
                    .Where(x => x.PushedDateTime.HasValue)
                    .ToArrayAsync();

                if (removed.Length > 0)
                {
                    var limit = DateTimeOffset.Now.Add(-EventLiveTime);
                    c.Set<EventEntity>().RemoveRange(removed.Where(x => x.PushedDateTime < limit).ToArray());
                }

                var events = await c.Set<EventEntity>()
                    .Where(x => eventIds.Contains(x.EventId) && !x.PushedDateTime.HasValue)
                    .ToArrayAsync();

                foreach (var @event in events)
                {
                    @event.PushedDateTime = DateTimeOffset.Now;
                }

                var result = mapper.Map<IEnumerable<EventValue>>(events);

                return result;
            }
        );

        lastWriteTimeUtc[file.FullName] = file.LastWriteTimeUtc;

        return result;*/
    }
}

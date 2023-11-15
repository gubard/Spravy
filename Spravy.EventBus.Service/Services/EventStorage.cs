using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spravy.Db.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Db.Contexts;
using Spravy.EventBus.Db.Models;
using Spravy.EventBus.Domain.Models;

namespace Spravy.EventBus.Service.Services;

public class EventStorage
{
    private static readonly TimeSpan EventLiveTime = TimeSpan.FromMinutes(1);
    private readonly IFactory<SpravyDbEventBusDbContext> dbContextFactory;
    private readonly IFactory<FileInfo> fileFactory;
    private readonly IMapper mapper;
    private static readonly Dictionary<string, DateTime> lastWriteTimeUtc = new();

    public EventStorage(
        IFactory<SpravyDbEventBusDbContext> dbContextFactory,
        IMapper mapper,
        IFactory<FileInfo> fileFactory
    )
    {
        this.dbContextFactory = dbContextFactory;
        this.mapper = mapper;
        this.fileFactory = fileFactory;
    }

    public async Task AddEventAsync(Guid id, byte[] content)
    {
        await using var context = dbContextFactory.Create();

        var newEvent = new EventEntity
        {
            EventId = id,
            Content = content,
            Id = Guid.NewGuid(),
        };

        await context.ExecuteSaveChangesTransactionAsync(
            c => c.Set<EventEntity>().AddAsync(newEvent)
        );
    }

    public async Task<IEnumerable<EventValue>> PushEventAsync(Guid[] eventIds)
    {
        return Enumerable.Empty<EventValue>();
        var file = fileFactory.Create();

        if (!lastWriteTimeUtc.TryGetValue(file.FullName, out var date))
        {
            lastWriteTimeUtc[file.FullName] = file.LastWriteTimeUtc;
        }
        else if (file.LastWriteTimeUtc == date)
        {
            return Enumerable.Empty<EventValue>();
        }

        await using var context = dbContextFactory.Create();

        var result = await context.ExecuteSaveChangesTransactionAsync(
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

        return result;
    }
}
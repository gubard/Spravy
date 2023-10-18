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
    private readonly IFactory<SpravyDbEventBusDbContext> dbContextFactory;
    private readonly IMapper mapper;

    public EventStorage(IFactory<SpravyDbEventBusDbContext> dbContextFactory, IMapper mapper)
    {
        this.dbContextFactory = dbContextFactory;
        this.mapper = mapper;
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
        await using var context = dbContextFactory.Create();

        return await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var events = await c.Set<EventEntity>()
                    .Where(x => eventIds.Contains(x.EventId))
                    .ToArrayAsync();

                var result = mapper.Map<IEnumerable<EventValue>>(events);
                c.Set<EventEntity>().RemoveRange(events);

                return result;
            }
        );
    }
}
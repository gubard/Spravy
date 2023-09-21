using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spravy.EventBus.Db.Contexts;
using Spravy.EventBus.Db.Models;
using Spravy.EventBus.Domain.Models;

namespace Spravy.EventBus.Service.Services;

public class EventStorage
{
    private readonly IDbContextFactory<SpravyEventBusDbContext> dbContextFactory;
    private readonly IMapper mapper;

    public EventStorage(IDbContextFactory<SpravyEventBusDbContext> dbContextFactory, IMapper mapper)
    {
        this.dbContextFactory = dbContextFactory;
        this.mapper = mapper;
    }

    public async Task AddEventAsync(Guid id, byte[] content)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        var newEvent = new EventEntity
        {
            EventId = id,
            Content = content,
            Id = Guid.NewGuid(),
        };

        await context.Set<EventEntity>().AddAsync(newEvent);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<EventValue>> PushEventAsync(Guid[] eventIds)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        await using  var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var events = await context.Set<EventEntity>()
                .Where(x => eventIds.Contains(x.EventId))
                .ToArrayAsync();

            var result = mapper.Map<IEnumerable<EventValue>>(events);
            context.Set<EventEntity>().RemoveRange(events);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }
}
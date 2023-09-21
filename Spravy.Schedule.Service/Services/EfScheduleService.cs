using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.Schedule.Db.Contexts;
using Spravy.Schedule.Db.Models;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Models;

namespace Spravy.Schedule.Service.Services;

public class EfScheduleService : IScheduleService
{
    private readonly IDbContextFactory<SpravyScheduleDbContext> dbContextFactory;
    private readonly IMapper mapper;

    public EfScheduleService(IMapper mapper, IDbContextFactory<SpravyScheduleDbContext> dbContextFactory)
    {
        this.mapper = mapper;
        this.dbContextFactory = dbContextFactory;
    }

    public async Task AddTimerAsync(AddTimerParameters parameters)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var newTimer = new TimerEntity
        {
            Id = Guid.NewGuid(),
            EventId = parameters.EventId,
            DueDateTime = parameters.DueDateTime,
            Content = parameters.Content,
        };

        await context.Set<TimerEntity>().AddAsync(newTimer);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TimerItem>> GetListTimesAsync()
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var timers = await context.Set<TimerEntity>().AsNoTracking().ToArrayAsync();
        var result = mapper.Map<IEnumerable<TimerItem>>(timers);

        return result;
    }

    public async Task RemoveTimerAsync(Guid id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var timer = await context.Set<TimerEntity>().FindAsync(id);
        context.Set<TimerEntity>().Remove(timer.ThrowIfNull());
        await context.SaveChangesAsync();
    }
}
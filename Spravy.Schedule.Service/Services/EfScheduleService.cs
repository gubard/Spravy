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
    private readonly SpravyScheduleDbContext context;
    private readonly IMapper mapper;

    public EfScheduleService(SpravyScheduleDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task AddTimerAsync(AddTimerParameters parameters)
    {
        var newTimer = new TimerEntity
        {
            Id = Guid.NewGuid(),
            EventId = parameters.EventId,
            DueDateTime = parameters.DueDateTime,
            Content = await parameters.Content.ToByteArrayAsync(),
        };

        await context.Set<TimerEntity>().AddAsync(newTimer);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TimerItem>> GetListTimesAsync()
    {
        var timers = await context.Set<TimerEntity>().AsNoTracking().ToArrayAsync();
        var result = mapper.Map<IEnumerable<TimerItem>>(timers);

        return result;
    }

    public async Task RemoveTimerAsync(Guid id)
    {
        var timer = await context.Set<TimerEntity>().FindAsync(id);
        context.Set<TimerEntity>().Remove(timer.ThrowIfNull());
        await context.SaveChangesAsync();
    }
}
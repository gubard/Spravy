using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spravy.Db.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Schedule.Db.Contexts;
using Spravy.Schedule.Db.Models;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Models;

namespace Spravy.Schedule.Service.Services;

public class EfScheduleService : IScheduleService
{
    private readonly IFactory<SpravyDbScheduleDbContext> dbContextFactory;
    private readonly IMapper mapper;

    public EfScheduleService(IMapper mapper, IFactory<SpravyDbScheduleDbContext> dbContextFactory)
    {
        this.mapper = mapper;
        this.dbContextFactory = dbContextFactory;
    }

    public async ValueTask<Result> AddTimerAsync(AddTimerParameters parameters, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        var newTimer = new TimerEntity
        {
            Id = Guid.NewGuid(),
            EventId = parameters.EventId,
            DueDateTime = parameters.DueDateTime,
            Content = parameters.Content,
        };

        await context.ExecuteSaveChangesTransactionAsync(
            c => c.Set<TimerEntity>().AddAsync(newTimer, cancellationToken)
        );

        return Result.Success;
    }

    public async ValueTask<Result<ReadOnlyMemory<TimerItem>>> GetListTimesAsync(CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var timers = await context.Set<TimerEntity>().AsNoTracking().ToArrayAsync(cancellationToken);
        var result = mapper.Map<ReadOnlyMemory<TimerItem>>(timers);

        return result.ToResult();
    }

    public async ValueTask<Result> RemoveTimerAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionValueAsync(
            async c =>
            {
                var timer = await context.Set<TimerEntity>().FindAsync(id);
                c.Set<TimerEntity>().Remove(timer.ThrowIfNull());
            }
        );

        return Result.Success;
    }
}
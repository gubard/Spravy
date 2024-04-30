using System.Runtime.CompilerServices;
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

    public ConfiguredValueTaskAwaitable<Result> AddTimerAsync(
        AddTimerParameters parameters,
        CancellationToken cancellationToken
    )
    {
        var newTimer = new TimerEntity
        {
            Id = Guid.NewGuid(),
            EventId = parameters.EventId,
            DueDateTime = parameters.DueDateTime,
            Content = parameters.Content,
        };

        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () => context.Set<TimerEntity>().AddEntityAsync(newTimer, cancellationToken).ToResultOnlyAsync(),
                    cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TimerItem>>> GetListTimesAsync(
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () => context.Set<TimerEntity>()
                       .AsNoTracking()
                       .ToArrayEntitiesAsync(cancellationToken)
                       .IfSuccessAsync(timers => mapper.Map<ReadOnlyMemory<TimerItem>>(timers).ToResult(),
                            cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> RemoveTimerAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () => context.FindEntityAsync<TimerEntity>(id)
                       .IfSuccessAsync(context.RemoveEntity, cancellationToken), cancellationToken), cancellationToken);
    }
}
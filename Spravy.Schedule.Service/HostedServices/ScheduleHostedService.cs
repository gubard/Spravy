using Microsoft.EntityFrameworkCore;
using Spravy.Db.Sqlite.Extensions;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.Schedule.Db.Contexts;
using Spravy.Schedule.Db.Models;

namespace Spravy.Schedule.Service.HostedServices;

public class ScheduleHostedService : IHostedService
{
    private readonly IFactory<string, SpravyScheduleDbContext> spravyScheduleDbContextFactory;
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly IFactory<string, IEventBusService> eventBusServiceFactory;

    public ScheduleHostedService(
        IFactory<string, SpravyScheduleDbContext> spravyScheduleDbContextFactory,
        SqliteFolderOptions sqliteFolderOptions,
        IFactory<string, IEventBusService> eventBusServiceFactory
    )
    {
        this.spravyScheduleDbContextFactory = spravyScheduleDbContextFactory;
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.eventBusServiceFactory = eventBusServiceFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var _executingTask = ExecuteAsync();

        if (_executingTask.IsCompleted)
        {
            return _executingTask;
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task ExecuteAsync()
    {
        while (true)
        {
            var dataBaseFiles = sqliteFolderOptions.GetDataBaseFiles();

            foreach (var dataBaseFile in dataBaseFiles)
            {
                await using var context =
                    spravyScheduleDbContextFactory.Create(dataBaseFile.ToSqliteConnectionString());

                var timers = await context.Set<TimerEntity>()
                    .AsNoTracking()
                    .ToArrayAsync();

                foreach (var timer in timers)
                {
                    if (timer.DueDateTime > DateTimeOffset.Now)
                    {
                        continue;
                    }

                    var eventBusService = eventBusServiceFactory.Create(dataBaseFile.GetFileNameWithoutExtension());
                    await eventBusService.PublishEventAsync(timer.EventId, timer.Content);
                    context.Set<TimerEntity>().Remove(timer);
                    await context.SaveChangesAsync();
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}
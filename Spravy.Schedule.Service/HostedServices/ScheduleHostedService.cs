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
    private readonly IFactory<string, SpravyDbScheduleDbContext> spravyScheduleDbContextFactory;
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly IFactory<string, IEventBusService> eventBusServiceFactory;
    private readonly ILogger<ScheduleHostedService> logger;
    private readonly Dictionary<string, Task> tasks = new();

    public ScheduleHostedService(
        IFactory<string, SpravyDbScheduleDbContext> spravyScheduleDbContextFactory,
        SqliteFolderOptions sqliteFolderOptions,
        IFactory<string, IEventBusService> eventBusServiceFactory,
        ILogger<ScheduleHostedService> logger
    )
    {
        this.spravyScheduleDbContextFactory = spravyScheduleDbContextFactory;
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.eventBusServiceFactory = eventBusServiceFactory;
        this.logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var dataBaseFiles = sqliteFolderOptions.GetDataBaseFiles();

        foreach (var dataBaseFile in dataBaseFiles)
        {
            tasks.Add(dataBaseFile.FullName, HandleDataBaseAsync(dataBaseFile));
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task HandleDataBaseAsync(FileInfo file)
    {
        while (true)
        {
            try
            {
                try
                {
                    await using var context =
                        spravyScheduleDbContextFactory.Create(file.ToSqliteConnectionString());

                    var timers = await context.Set<TimerEntity>()
                        .AsNoTracking()
                        .ToArrayAsync();

                    foreach (var timer in timers)
                    {
                        if (timer.DueDateTime > DateTimeOffset.Now)
                        {
                            continue;
                        }

                        var eventBusService = eventBusServiceFactory.Create(file.GetFileNameWithoutExtension());
                        await eventBusService.PublishEventAsync(timer.EventId, timer.Content);
                        context.Set<TimerEntity>().Remove(timer);
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {
                    throw new FileException($"Can't handle file {file}.", e, file);
                }
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e, null);
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
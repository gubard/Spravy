namespace Spravy.Schedule.Service.HostedServices;

public class ScheduleHostedService : IHostedService
{
    private readonly IFactory<string, IEventBusService> eventBusServiceFactory;
    private readonly ILogger<ScheduleHostedService> logger;
    private readonly IFactory<string, SpravyDbScheduleDbContext> spravyScheduleDbContextFactory;
    private readonly SqliteFolderOptions sqliteFolderOptions;
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

    public Task StartAsync(CancellationToken ct)
    {
        var dataBaseFiles = sqliteFolderOptions.GetDataBaseFiles();

        foreach (var dataBaseFile in dataBaseFiles)
        {
            tasks.Add(dataBaseFile.FullName, HandleDataBaseAsync(dataBaseFile, ct));
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    private async Task HandleDataBaseAsync(FileInfo file, CancellationToken ct)
    {
        while (true)
        {
            try
            {
                try
                {
                    await spravyScheduleDbContextFactory
                        .Create(file.ToSqliteConnectionString())
                        .IfSuccessAsync(
                            context =>
                                context.AtomicExecuteAsync(
                                    () =>
                                        context
                                            .Set<TimerEntity>()
                                            .AsNoTracking()
                                            .ToArrayEntitiesAsync(ct)
                                            .IfSuccessAllInOrderAsync(
                                                timers =>
                                                {
                                                    var list = new List<Func<Cvtar>>();

                                                    foreach (var timer in timers.Span)
                                                    {
                                                        if (timer.DueDateTime > DateTimeOffset.Now)
                                                        {
                                                            continue;
                                                        }

                                                        list.Add(
                                                            () =>
                                                                eventBusServiceFactory
                                                                    .Create(
                                                                        file.GetFileNameWithoutExtension()
                                                                    )
                                                                    .IfSuccessAsync(
                                                                        eventBusService =>
                                                                            eventBusService
                                                                                .PublishEventAsync(
                                                                                    timer.EventId,
                                                                                    timer.Content,
                                                                                    ct
                                                                                )
                                                                                .IfSuccessAsync(
                                                                                    () =>
                                                                                        context.RemoveEntity(
                                                                                            timer
                                                                                        ),
                                                                                    ct
                                                                                ),
                                                                        ct
                                                                    )
                                                        );
                                                    }

                                                    return list.ToArray();
                                                },
                                                ct
                                            ),
                                    ct
                                ),
                            ct
                        );
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

            await Task.Delay(TimeSpan.FromSeconds(1), ct);
        }
    }
}

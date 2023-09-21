using Spravy.Db.Sqlite.Extensions;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Domain.Helpers;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Protos;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;

namespace Spravy.ToDo.Service.HostedServices;

public class EventBusHostedService : IHostedService
{
    private static readonly ReadOnlyMemory<Guid> eventIds;
    private readonly IFactory<string, SpravyToDoDbContext> spravyToDoDbContext;
    private readonly IFactory<string, IEventBusService> spravyEventBusServiceFactory;
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly Dictionary<string, Task> tasks = new();
    private readonly ILogger<EventBusHostedService> logger;

    static EventBusHostedService()
    {
        eventIds = new[]
        {
            EventIdHelper.ChangeCurrentId
        };
    }

    public EventBusHostedService(
        IFactory<string, SpravyToDoDbContext> spravyToDoDbContext,
        SqliteFolderOptions sqliteFolderOptions,
        IFactory<string, IEventBusService> spravyEventBusServiceFactory,
        ILogger<EventBusHostedService> logger
    )
    {
        this.spravyToDoDbContext = spravyToDoDbContext;
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.spravyEventBusServiceFactory = spravyEventBusServiceFactory;
        this.logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Execute();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void Execute()
    {
        var files = sqliteFolderOptions.GetDataBaseFiles();

        foreach (var file in files)
        {
            var task = HandleEventsAsync(file);
            tasks.Add(file.FullName, task);
        }
    }

    private async Task HandleEventsAsync(FileInfo file)
    {
        try
        {
            try
            {
                while (true)
                {
                    var eventBusService = spravyEventBusServiceFactory.Create(file.GetFileNameWithoutExtension());
                    await using var context = spravyToDoDbContext.Create(file.ToSqliteConnectionString());
                    var events = await eventBusService.GetEventsAsync(eventIds);

                    foreach (var @event in events)
                    {
                        if (@event.Id == EventIdHelper.ChangeCurrentId)
                        {
                            var eventContent = ChangeToDoItemIsCurrentEvent.Parser.ParseFrom(@event.Content);

                            var item = await context.Set<ToDoItemEntity>()
                                .FindAsync(new Guid(eventContent.ToDoItemId.ToByteArray()));

                            item.ThrowIfNull().IsCurrent = eventContent.IsCurrent;
                            await context.SaveChangesAsync();
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(20));
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
            await Task.Delay(TimeSpan.FromSeconds(30));
            tasks[file.FullName] = HandleEventsAsync(file);
        }
    }
}
using Spravy.Db.Extensions;
using Spravy.Db.Sqlite.Extensions;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.EventBus.Domain.Helpers;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Protos;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;

namespace Spravy.ToDo.Service.HostedServices;

public class EventBusHostedService : IHostedService
{
    private static readonly ReadOnlyMemory<Guid> eventIds;
    private readonly ILogger<EventBusHostedService> logger;
    private readonly IFactory<string, IEventBusService> spravyEventBusServiceFactory;
    private readonly IFactory<string, SpravyDbToDoDbContext> spravyToDoDbContext;
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly Dictionary<string, Task> tasks = new();

    static EventBusHostedService()
    {
        eventIds = new[]
        {
            EventIdHelper.ChangeFavoriteId,
        };
    }

    public EventBusHostedService(
        IFactory<string, SpravyDbToDoDbContext> spravyToDoDbContext,
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
        while (true)
        {
            try
            {
                var source = new CancellationTokenSource();

                try
                {
                    var eventBusService = spravyEventBusServiceFactory.Create(file.GetFileNameWithoutExtension())
                       .ThrowIfError();

                    var stream = eventBusService.SubscribeEventsAsync(eventIds.ToArray(), source.Token);
                    logger.LogInformation("Connected for events {File}", file);

                    await foreach (var eventValue in stream)
                    {
                        if (eventValue.ThrowIfError().Id == EventIdHelper.ChangeFavoriteId)
                        {
                            await ChangeFavoriteAsync(file, eventValue.Value);
                        }
                    }
                }
                catch (Exception e)
                {
                    await source.CancelAsync();

                    throw new FileException($"Can't handle file {file}.", e, file);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Handle file {File}", file);
            }

            await Task.Delay(TimeSpan.FromSeconds(20));
        }
    }

    private async Task ChangeFavoriteAsync(FileInfo file, EventValue eventValue)
    {
        var eventContent = ChangeToDoItemIsFavoriteEvent.Parser.ParseFrom(eventValue.Content);
        var id = new Guid(eventContent.ToDoItemId.ToByteArray());

        await spravyToDoDbContext.Create(file.ToSqliteConnectionString())
           .IfSuccessAsync(context => context.AtomicExecuteAsync(() => context.FindEntityAsync<ToDoItemEntity>(id)
               .IfSuccessAsync(item =>
                {
                    item.IsFavorite = eventContent.IsFavorite;

                    return Result.Success;
                }, CancellationToken.None), CancellationToken.None), CancellationToken.None)
           .ThrowIfErrorAsync();
    }
}
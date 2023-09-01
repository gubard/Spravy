using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Domain.Helpers;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Protos;
using Spravy.ToDo.Db.Contexts;

namespace Spravy.ToDo.Service.HostedServices;

public class EventSubscriberHostedService : IHostedService
{
    private readonly IEventBusService eventBusService;
    private readonly IFactory<string, SpravyToDoDbContext> spravyToDoDbContextFactory;
    private readonly SqliteFolderOptions sqliteFolderOptions;

    private static readonly Guid[] EventIds =
    {
        EventIdHelper.CreateUserId
    };

    public EventSubscriberHostedService(
        IEventBusService eventBusService,
        IFactory<string, SpravyToDoDbContext> spravyToDoDbContextFactory,
        SqliteFolderOptions sqliteFolderOptions
    )
    {
        this.eventBusService = eventBusService;
        this.spravyToDoDbContextFactory = spravyToDoDbContextFactory;
        this.sqliteFolderOptions = sqliteFolderOptions;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        SubscribeAsync(cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task SubscribeAsync(CancellationToken cancellationToken)
    {
        var eventsPipeline = eventBusService.SubscribeEventsAsync(EventIds, cancellationToken);

        await foreach (var eventValue in eventsPipeline)
        {
            if (eventValue.Id == EventIdHelper.CreateUserId)
            {
                var eventContent = CreateUserEvent.Parser.ParseFrom(eventValue.Stream);
                var jwtToken = new JwtSecurityToken(eventContent.Token);
                var claimId = jwtToken.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier);

                var dataBaseFile = sqliteFolderOptions.DataBasesFolder.ThrowIfNull()
                    .ToDirectory()
                    .ToFile($"{Guid.Parse(claimId.Value)}.db");

                await using var context = spravyToDoDbContextFactory.Create($"DataSource={dataBaseFile}");
                await context.Database.MigrateAsync(cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Domain.Helpers;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Protos;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Service.Models;

namespace Spravy.ToDo.Service.HostedServices;

public class EventSubscriberHostedService : IHostedService
{
    private readonly IEventBusService eventBusService;
    private readonly IFactory<string, SpravyToDoDbContext> spravyToDoDbContextFactory;
    private readonly SqliteOptions sqliteOptions;

    private static readonly Guid[] EventIds =
    {
        EventIdHelper.CreateUserId
    };

    public EventSubscriberHostedService(
        IEventBusService eventBusService,
        IFactory<string, SpravyToDoDbContext> spravyToDoDbContextFactory,
        SqliteOptions sqliteOptions
    )
    {
        this.eventBusService = eventBusService;
        this.spravyToDoDbContextFactory = spravyToDoDbContextFactory;
        this.sqliteOptions = sqliteOptions;
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

                var dataBaseFile = sqliteOptions.DataBasesFolder.ToDirectory()
                    .ToFile($"{Guid.Parse(claimId.Value)}.db");

                await using var context = spravyToDoDbContextFactory.Create($"DataSource={dataBaseFile}");
                await context.Database.EnsureCreatedAsync(cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
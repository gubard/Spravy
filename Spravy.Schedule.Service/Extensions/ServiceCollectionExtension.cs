using Spravy.Authentication.Domain.Client.Models;
using Spravy.Authentication.Domain.Client.Services;
using Spravy.Authentication.Domain.Services;
using Spravy.Authentication.Protos;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Core.Extensions;
using Spravy.Core.Services;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Client.Services;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Protos;
using Spravy.Schedule.Db.Contexts;
using Spravy.Schedule.Db.Sqlite.Migrator;
using Spravy.Schedule.Db.Sqlite.Services;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Service.Services;
using Spravy.Service.Extensions;
using Spravy.Service.HostedServices;
using Spravy.Service.Services;
using IAuthenticationService = Spravy.Authentication.Domain.Interfaces.IAuthenticationService;

namespace Spravy.Schedule.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterSchedule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<FolderMigratorHostedService<SpravyDbScheduleDbContext>>();
        serviceCollection.AddSpravySqliteFolderContext<SpravyDbScheduleDbContext, SpravyScheduleDbSqliteMigratorMark>();
        serviceCollection.AddSingleton<ITokenService, TokenService>();
        serviceCollection.AddSingleton<IFactory<string, SpravyDbScheduleDbContext>, SpravyScheduleDbContextFactory>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteScheduleDbContextSetup>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddSingleton<IFactory<string, IEventBusService>, EventBusServiceFactory>();
        //serviceCollection.AddSingleton<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();
        serviceCollection.AddSingleton<ITokenService, TokenService>();
        serviceCollection.AddSingleton<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<ContextAccessorUserIdHttpHeaderFactory>();
        serviceCollection.AddSingleton<TimeZoneHttpHeaderFactory>();
        serviceCollection.AddTransient<IHttpHeaderFactory, TimeZoneHttpHeaderFactory>();
        serviceCollection.AddTransient<ISerializer, ProtobufSerializer>();
        serviceCollection.AddTransient<IScheduleService, EfScheduleService>();

        serviceCollection
           .AddGrpcService<GrpcAuthenticationService, AuthenticationService.AuthenticationServiceClient,
                GrpcAuthenticationServiceOptions>();

        serviceCollection
           .AddGrpcServiceAuth<GrpcEventBusService, EventBusService.EventBusServiceClient,
                GrpcEventBusServiceOptions>();

        serviceCollection.AddSingleton<IAuthenticationService>(sp =>
            sp.GetRequiredService<GrpcAuthenticationService>());

        return serviceCollection;
    }
}
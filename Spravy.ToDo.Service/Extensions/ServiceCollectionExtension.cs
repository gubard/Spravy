using Spravy.Authentication.Domain.Client.Models;
using Spravy.Authentication.Domain.Client.Services;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Mapper.Profiles;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Domain.Services;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Models;
using Spravy.Di.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Client.Services;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Domain.Mapper.Profiles;
using Spravy.Service.Extensions;
using Spravy.Service.HostedServices;
using Spravy.Service.Services;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Mapper.Profiles;
using Spravy.ToDo.Db.Services;
using Spravy.ToDo.Db.Sqlite.Migrator;
using Spravy.ToDo.Db.Sqlite.Services;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Mapper.Profiles;
using Spravy.ToDo.Service.HostedServices;
using Spravy.ToDo.Service.Services;
using Spravy.Client.Extensions;
using Spravy.EventBus.Protos;

namespace Spravy.ToDo.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddToDo(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<MigratorHostedService<SpravyDbToDoDbContext>>();
        serviceCollection.AddHostedService<EventBusHostedService>();
        serviceCollection.AddSpravySqliteFolderContext<SpravyDbToDoDbContext, SpravyToDoDbSqliteMigratorMark>();
        serviceCollection
            .AddMapperConfiguration<SpravyToDoProfile, SpravyToDoDbProfile, SpravyEventBusProfile,
                SpravyAuthenticationProfile>();
        serviceCollection
            .AddGrpcService2<GrpcAuthenticationService,
                Spravy.Authentication.Protos.AuthenticationService.AuthenticationServiceClient,
                GrpcAuthenticationServiceOptions>();
        serviceCollection
            .AddGrpcService<GrpcEventBusService, EventBusService.EventBusServiceClient, GrpcEventBusServiceOptions>();

        serviceCollection.AddSingleton<ITokenService, TokenService>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteToDoDbContextSetup>();
        serviceCollection.AddSingleton<IFactory<string, SpravyDbToDoDbContext>, SpravyToDoDbContextFactory>();
        serviceCollection.AddSingleton<IEventBusService>(sp => sp.GetRequiredService<GrpcEventBusService>());
        serviceCollection.AddSingleton<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();

        serviceCollection.AddSingleton<IAuthenticationService>(
            sp => sp.GetRequiredService<GrpcAuthenticationService>()
        );

        serviceCollection.AddSingleton<IFactory<string, IEventBusService>, EventBusServiceFactory>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddSingleton<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<ContextAccessorHttpHeaderFactory>();
        serviceCollection.AddSingleton<IHttpHeaderFactory, EmptyHttpHeaderFactory>();

        serviceCollection.AddTransient<IToDoService, EfToDoService>();
        serviceCollection.AddTransient<StatusToDoItemService>();
        serviceCollection.AddTransient<ActiveToDoItemToDoItemService>();

        return serviceCollection;
    }
}
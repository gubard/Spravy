using System.Text.Json.Serialization;
using Grpc.Core;
using Spravy.Authentication.Domain.Client.Services;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Services;
using Spravy.Authentication.Protos;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Core.Extensions;
using Spravy.Core.Interfaces;
using Spravy.Core.Services;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.Service.Extensions;
using Spravy.Service.HostedServices;
using Spravy.Service.Services;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Services;
using Spravy.ToDo.Db.Sqlite.Migrator;
using Spravy.ToDo.Db.Sqlite.Services;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Service.Services;

namespace Spravy.ToDo.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterToDo(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<FolderMigratorHostedService<SpravyDbToDoDbContext>>();
        //serviceCollection.AddHostedService<EventBusHostedService>();
        serviceCollection.AddSpravySqliteFolderContext<
            SpravyDbToDoDbContext,
            SpravyToDoDbSqliteMigratorMark
        >();
        serviceCollection.AddSingleton<ITokenService, TokenService>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteToDoDbContextSetup>();
        serviceCollection.AddSingleton<
            IFactory<string, SpravyDbToDoDbContext>,
            SpravyToDoDbContextFactory
        >();
        serviceCollection.AddTransient<
            IFactory<ChannelBase, AuthenticationService.AuthenticationServiceClient>,
            AuthenticationClientFactory
        >();
        serviceCollection.AddTransient<IRpcExceptionHandler, RpcExceptionHandler>();
        //serviceCollection.AddSingleton<IEventBusService>(sp => sp.GetRequiredService<GrpcEventBusService>());
        //serviceCollection.AddSingleton<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();
        //serviceCollection.AddSingleton<IFactory<string, IEventBusService>, EventBusServiceFactory>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddSingleton<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<ContextAccessorUserIdHttpHeaderFactory>();
        serviceCollection.AddSingleton<TimeZoneHttpHeaderFactory>();
        serviceCollection.AddTransient<IToDoService, EfToDoService>();
        serviceCollection.AddTransient<GetterToDoItemParametersService>();
        serviceCollection.AddTransient<ISerializer, SpravyJsonSerializer>();
        serviceCollection.AddTransient<JsonSerializerContext, SpravyJsonSerializerContext>();

        /*serviceCollection.AddGrpcService<GrpcAuthenticationService,
            Spravy.Authentication.Protos.AuthenticationService.AuthenticationServiceClient,
            GrpcAuthenticationServiceOptions>();

        serviceCollection.AddGrpcServiceAuth<GrpcEventBusService,
            EventBusService.EventBusServiceClient,
            GrpcEventBusServiceOptions>();*/

        serviceCollection.AddSingleton<IAuthenticationService>(sp =>
            sp.GetRequiredService<GrpcAuthenticationService>()
        );

        serviceCollection.AddSingleton<IHttpHeaderFactory>(sp => new CombineHttpHeaderFactory(
            sp.GetRequiredService<TimeZoneHttpHeaderFactory>(),
            sp.GetRequiredService<ContextAccessorUserIdHttpHeaderFactory>()
        ));

        return serviceCollection;
    }
}

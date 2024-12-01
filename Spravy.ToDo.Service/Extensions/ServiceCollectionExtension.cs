using Spravy.Client.Extensions;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Client.Services;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Protos;

namespace Spravy.ToDo.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterToDo(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IRetryService, RetryService>();
        serviceCollection.AddHostedService<FolderMigratorHostedService<SpravyDbToDoDbContext>>();
        serviceCollection.AddSpravySqliteFolderContext<SpravyDbToDoDbContext, SpravyToDoDbSqliteMigratorMark>();
        serviceCollection.AddSingleton<ITokenService, TokenService>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteToDoDbContextSetup>();
        serviceCollection.AddSingleton<IFactory<string, SpravyDbToDoDbContext>, SpravyToDoDbContextFactory>();

        serviceCollection
           .AddTransient<IFactory<ChannelBase, AuthenticationService.AuthenticationServiceClient>,
                AuthenticationClientFactory>();

        serviceCollection.AddTransient<IRpcExceptionHandler, RpcExceptionHandler>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddSingleton<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<ContextAccessorUserIdHttpHeaderFactory>();
        serviceCollection.AddSingleton<TimeZoneHttpHeaderFactory>();
        serviceCollection.AddTransient<IToDoService, EfToDoService>();
        serviceCollection.AddTransient<GetterToDoItemParametersService>();
        serviceCollection.AddTransient<ISerializer, SpravyJsonSerializer>();
        serviceCollection.AddTransient<ContextAccessorAuthorizationHttpHeaderFactory>();

        serviceCollection
           .AddTransient<IFactory<ChannelBase, EventBusService.EventBusServiceClient>, EventBusServiceClientFactory>();

        serviceCollection.AddTransient<IEventBusService>(sp => sp.GetRequiredService<GrpcEventBusService>());
        serviceCollection.AddTransient<JsonSerializerContext, SpravyJsonSerializerContext>();

        serviceCollection.AddSingleton<IAuthenticationService>(
            sp => sp.GetRequiredService<GrpcAuthenticationService>()
        );

        serviceCollection.AddTransient<IHttpHeaderFactory>(
            sp => new CombineHttpHeaderFactory(
                sp.GetRequiredService<ContextAccessorUserIdHttpHeaderFactory>(),
                sp.GetRequiredService<TimeZoneHttpHeaderFactory>(),
                sp.GetRequiredService<ContextAccessorAuthorizationHttpHeaderFactory>()
            )
        );

        serviceCollection
           .AddGrpcServiceAuth<GrpcEventBusService, EventBusService.EventBusServiceClient,
                GrpcEventBusServiceOptions>();

        return serviceCollection;
    }
}
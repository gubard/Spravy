namespace Spravy.Schedule.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterSchedule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IRetryService, RetryService>();
        serviceCollection.AddHostedService<FolderMigratorHostedService<SpravyDbScheduleDbContext>>();
        serviceCollection.AddSpravySqliteFolderContext<SpravyDbScheduleDbContext, SpravyScheduleDbSqliteMigratorMark>();
        serviceCollection.AddSingleton<IFactory<string, SpravyDbScheduleDbContext>, SpravyScheduleDbContextFactory>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteScheduleDbContextSetup>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddSingleton<IFactory<string, IEventBusService>, EventBusServiceFactory>();

        serviceCollection
           .AddTransient<IFactory<ChannelBase, AuthenticationService.AuthenticationServiceClient>,
                AuthenticationClientFactory>();

        serviceCollection.AddTransient<IRpcExceptionHandler, RpcExceptionHandler>();
        serviceCollection.AddSingleton<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<ContextAccessorUserIdHttpHeaderFactory>();
        serviceCollection.AddSingleton<TimeZoneHttpHeaderFactory>();
        serviceCollection.AddSingleton<ContextAccessorAuthorizationHttpHeaderFactory>();

        serviceCollection.AddTransient<IHttpHeaderFactory>(
            sp => new CombineHttpHeaderFactory(
                sp.GetRequiredService<ContextAccessorUserIdHttpHeaderFactory>(),
                sp.GetRequiredService<TimeZoneHttpHeaderFactory>(),
                sp.GetRequiredService<ContextAccessorAuthorizationHttpHeaderFactory>()
            )
        );

        serviceCollection.AddTransient<IScheduleService, EfScheduleService>();
        serviceCollection.AddTransient<ISerializer, SpravyJsonSerializer>();
        serviceCollection.AddTransient<JsonSerializerContext, SpravyJsonSerializerContext>();

        serviceCollection
           .AddTransient<IFactory<ChannelBase, EventBusService.EventBusServiceClient>, EventBusServiceClientFactory>();

        serviceCollection.AddTransient<IEventBusService>(sp => sp.GetRequiredService<GrpcEventBusService>());

        serviceCollection
           .AddGrpcService<GrpcAuthenticationService, AuthenticationService.AuthenticationServiceClient,
                GrpcAuthenticationServiceOptions>();

        serviceCollection
           .AddGrpcServiceAuth<GrpcEventBusService, EventBusService.EventBusServiceClient,
                GrpcEventBusServiceOptions>();

        serviceCollection.AddSingleton<IAuthenticationService>(
            sp => sp.GetRequiredService<GrpcAuthenticationService>()
        );

        return serviceCollection;
    }
}
namespace Spravy.Schedule.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterSchedule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IRetryService, RetryService>();
        serviceCollection.AddHostedService<
            FolderMigratorHostedService<SpravyDbScheduleDbContext>
        >();
        serviceCollection.AddSpravySqliteFolderContext<
            SpravyDbScheduleDbContext,
            SpravyScheduleDbSqliteMigratorMark
        >();
        serviceCollection.AddSingleton<ITokenService, TokenService>();
        serviceCollection.AddSingleton<
            IFactory<string, SpravyDbScheduleDbContext>,
            SpravyScheduleDbContextFactory
        >();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteScheduleDbContextSetup>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddSingleton<
            IFactory<string, IEventBusService>,
            EventBusServiceFactory
        >();
        serviceCollection.AddTransient<
            IFactory<ChannelBase, AuthenticationService.AuthenticationServiceClient>,
            AuthenticationClientFactory
        >();
        serviceCollection.AddTransient<IRpcExceptionHandler, RpcExceptionHandler>();
        //serviceCollection.AddSingleton<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();
        serviceCollection.AddSingleton<ITokenService, TokenService>();
        serviceCollection.AddSingleton<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<ContextAccessorUserIdHttpHeaderFactory>();
        serviceCollection.AddSingleton<TimeZoneHttpHeaderFactory>();
        serviceCollection.AddTransient<IHttpHeaderFactory, TimeZoneHttpHeaderFactory>();
        serviceCollection.AddTransient<IScheduleService, EfScheduleService>();
        serviceCollection.AddTransient<ISerializer, SpravyJsonSerializer>();
        serviceCollection.AddTransient<JsonSerializerContext, SpravyJsonSerializerContext>();

        serviceCollection.AddGrpcService<
            GrpcAuthenticationService,
            AuthenticationService.AuthenticationServiceClient,
            GrpcAuthenticationServiceOptions
        >();

        serviceCollection.AddGrpcServiceAuth<
            GrpcEventBusService,
            EventBusService.EventBusServiceClient,
            GrpcEventBusServiceOptions
        >();

        serviceCollection.AddSingleton<IAuthenticationService>(sp =>
            sp.GetRequiredService<GrpcAuthenticationService>()
        );

        return serviceCollection;
    }
}

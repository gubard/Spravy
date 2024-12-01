namespace Spravy.EventBus.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterEventBus(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<FolderMigratorHostedService<SpravyDbEventBusDbContext>>();
        serviceCollection.AddSpravySqliteFolderContext<SpravyDbEventBusDbContext, SpravyEventBusDbSqliteMigratorMark>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteEventBusDbContextSetup>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddTransient<EventStorage>();
        serviceCollection.AddTransient<IRpcExceptionHandler, RpcExceptionHandler>();
        serviceCollection.AddTransient<ISerializer, SpravyJsonSerializer>();
        serviceCollection.AddTransient<JsonSerializerContext, SpravyJsonSerializerContext>();

        return serviceCollection;
    }
}
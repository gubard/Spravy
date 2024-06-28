using System.Text.Json.Serialization;
using Spravy.Core.Extensions;
using Spravy.Core.Interfaces;
using Spravy.Core.Services;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Db.Contexts;
using Spravy.EventBus.Db.Sqlite.Migrator;
using Spravy.EventBus.Db.Sqlite.Services;
using Spravy.EventBus.Service.Services;
using Spravy.Service.Extensions;
using Spravy.Service.HostedServices;

namespace Spravy.EventBus.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterEventBus(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<
            FolderMigratorHostedService<SpravyDbEventBusDbContext>
        >();
        serviceCollection.AddSpravySqliteFolderContext<
            SpravyDbEventBusDbContext,
            SpravyEventBusDbSqliteMigratorMark
        >();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteEventBusDbContextSetup>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddTransient<EventStorage>();
        serviceCollection.AddTransient<IRpcExceptionHandler, RpcExceptionHandler>();
        serviceCollection.AddTransient<ISerializer, SpravyJsonSerializer>();
        serviceCollection.AddTransient<JsonSerializerContext, SpravyJsonSerializerContext>();

        return serviceCollection;
    }
}

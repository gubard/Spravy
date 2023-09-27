using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Db.Contexts;
using Spravy.EventBus.Db.Mapper.Profiles;
using Spravy.EventBus.Db.Sqlite.Migrator;
using Spravy.EventBus.Db.Sqlite.Services;
using Spravy.EventBus.Domain.Mapper.Profiles;
using Spravy.EventBus.Service.Services;
using Spravy.Service.Extensions;
using Spravy.Service.HostedServices;

namespace Spravy.EventBus.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddEventBus(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<MigratorHostedService<SpravyEventBusDbContext>>();

        serviceCollection.AddSingleton<IFactory<string, SpravyEventBusDbContext>, SpravyEventBusDbContextFactory>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteEventBusDbContextSetup>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());

        serviceCollection.AddTransient<EventStorage>();

        serviceCollection.AddMapperConfiguration<SpravyEventBusProfile, SpravyEventBusDbProfile>();
        serviceCollection.AddSpravySqliteFolderContext<SpravyEventBusDbContext>();

        return serviceCollection;
    }
}
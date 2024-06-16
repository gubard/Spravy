using Spravy.Core.Extensions;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Models;
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
    public static IServiceCollection RegisterEventBus(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<FolderMigratorHostedService<SpravyDbEventBusDbContext>>();
        serviceCollection.AddMapperConfiguration<SpravyEventBusProfile, SpravyEventBusDbProfile>();
        serviceCollection.AddSpravySqliteFolderContext<SpravyDbEventBusDbContext, SpravyEventBusDbSqliteMigratorMark>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteEventBusDbContextSetup>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddTransient<EventStorage>();

        return serviceCollection;
    }
}
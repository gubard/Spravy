using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Extensions;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Interfaces;
using Spravy.Schedule.Db.Contexts;
using Spravy.Schedule.Db.Sqlite.Migrator;
using Spravy.Schedule.Db.Sqlite.Services;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Mapper.Profiles;
using Spravy.Schedule.Service.HostedServices;
using Spravy.Schedule.Service.Services;
using Spravy.Service.Extensions;

namespace Spravy.Schedule.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSchedule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMapperConfiguration<SpravyScheduleProfile>();
        serviceCollection.AddSpravyDbContext<SpravyScheduleDbContext>();
        serviceCollection.AddSingleton<IFactory<string, SpravyScheduleDbContext>, SpravyScheduleDbContextFactory>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteScheduleDbContextSetup>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFileOptions>());
        serviceCollection.AddHostedService<MigratorHostedService>();
        serviceCollection.AddScoped<IScheduleService, EfScheduleService>();

        return serviceCollection;
    }
}
using Spravy.Db.Sqlite.Models;
using Spravy.Di.Extensions;
using Spravy.PasswordGenerator.Db.Contexts;
using Spravy.PasswordGenerator.Db.Mapper.Profiles;
using Spravy.PasswordGenerator.Db.Sqlite.Migrator;
using Spravy.PasswordGenerator.Db.Sqlite.Services;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Mapper.Profiles;
using Spravy.PasswordGenerator.Service.Services;
using Spravy.Service.Extensions;
using Spravy.Service.HostedServices;

namespace Spravy.PasswordGenerator.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterPasswordGenerator(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<MigratorHostedService<PasswordDbContext>>();
        serviceCollection.AddTransient<IPasswordService, EfPasswordService>();
        serviceCollection.AddTransient<SqlitePasswordDbContextSetup>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddMapperConfiguration<SpravyPasswordGeneratorDbProfile, SpravyPasswordGeneratorProfile>();

        serviceCollection.AddSpravySqliteFolderContext<
            PasswordDbContext,
            SpravyPasswordGeneratorDbSqliteMigratorMark,
            SqlitePasswordDbContextSetup
        >();

        return serviceCollection;
    }
}
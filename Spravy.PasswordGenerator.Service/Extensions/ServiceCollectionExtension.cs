using Spravy.Core.Services;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Extensions;
using Spravy.Db.Sqlite.Models;
using Spravy.Di.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.PasswordGenerator.Db.Contexts;
using Spravy.PasswordGenerator.Db.Mapper.Profiles;
using Spravy.PasswordGenerator.Db.Sqlite.Migrator;
using Spravy.PasswordGenerator.Db.Sqlite.Services;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Mapper.Profiles;
using Spravy.PasswordGenerator.Domain.Models;
using Spravy.PasswordGenerator.Service.Services;
using Spravy.Service.Extensions;
using Spravy.Service.HostedServices;

namespace Spravy.PasswordGenerator.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterPasswordGenerator(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<FolderMigratorHostedService<PasswordDbContext>>();
        serviceCollection.AddHostedService<FileMigratorHostedService<UserSecretDbContext>>();
        serviceCollection.AddTransient<IPasswordService, EfPasswordService>();
        serviceCollection.AddTransient<ISerializer, ProtobufSerializer>();
        serviceCollection.AddTransient<SqlitePasswordDbContextSetup>();
        serviceCollection.AddTransient<IDbContextSetup, SqliteUserSecretDbContextSetup>();
        serviceCollection.AddTransient<IUserSecretService, EfUserSecretService>();
        serviceCollection.AddTransient<IPasswordGenerator, Domain.Services.PasswordGenerator>();
        serviceCollection.AddTransient<IStringToBytes, StringToUtf8Bytes>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFileOptions>());
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<PasswordGeneratorOptions>());
        serviceCollection.AddMapperConfiguration<SpravyPasswordGeneratorDbProfile, SpravyPasswordGeneratorProfile>();
        serviceCollection.AddTransient<IFactory<string, UserSecretDbContext>, UserSecretDbContextFactory>();

        serviceCollection
           .AddSpravySqliteFolderContext<PasswordDbContext, SpravyPasswordGeneratorDbSqliteMigratorMark,
                SqlitePasswordDbContextSetup>();

        serviceCollection
           .AddSpravySqliteFileDbContext<UserSecretDbContext, SpravyPasswordGeneratorDbSqliteMigratorMark>();

        return serviceCollection;
    }
}
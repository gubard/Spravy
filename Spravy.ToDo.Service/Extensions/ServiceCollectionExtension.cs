using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Spravy.Authentication.Domain.Models;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Client.Services;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Domain.Mapper.Profiles;
using Spravy.Service.Extensions;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Mapper.Profiles;
using Spravy.ToDo.Db.Sqlite.Migrator;
using Spravy.ToDo.Db.Sqlite.Services;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Mapper.Profiles;
using Spravy.ToDo.Service.HostedServices;
using Spravy.ToDo.Service.Services;

namespace Spravy.ToDo.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddToDo(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMapperConfiguration<SpravyToDoProfile, SpravyToDoDbProfile, SpravyEventBusProfile>();
        serviceCollection.AddScoped<IToDoService, EfToDoService>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteToDoDbContextSetup>();
        serviceCollection.AddHostedService<MigratorHostedService>();
        serviceCollection.AddHostedService<EventSubscriberHostedService>();
        serviceCollection.AddSingleton<IFactory<string, SpravyToDoDbContext>, SpravyToDoDbContextFactory>();
        serviceCollection.AddSingleton<IEventBusService, GrpcEventBusService>();
        serviceCollection.AddSingleton<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<GrpcEventBusServiceOptions>());
        serviceCollection.AddSpravyToDoDbContext();

        return serviceCollection;
    }

    public static IServiceCollection AddSpravyToDoDbContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContextFactory<SpravyToDoDbContext>(
            (sp, options) =>
            {
                var sqliteOptions = sp.GetRequiredService<SqliteFolderOptions>();
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var claims = httpContextAccessor.HttpContext.ThrowIfNull().User.Claims;
                var nameIdentifier = claims.Single(x => x.Type == ClaimTypes.NameIdentifier);
                var fileName = $"{nameIdentifier.Value}.db";
                var dataBasesFolder = sqliteOptions.DataBasesFolder.ThrowIfNull();
                var connectionString = $"DataSource={Path.Combine(dataBasesFolder, fileName)}";
                options.UseSqlite(connectionString);
            }
        );

        return serviceCollection;
    }
}
using Spravy.Authentication.Domain.Client.Models;
using Spravy.Authentication.Domain.Client.Services;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Domain.Services;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Client.Services;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.Schedule.Db.Contexts;
using Spravy.Schedule.Db.Mapper.Profiles;
using Spravy.Schedule.Db.Sqlite.Migrator;
using Spravy.Schedule.Db.Sqlite.Services;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Mapper.Profiles;
using Spravy.Schedule.Service.HostedServices;
using Spravy.Schedule.Service.Services;
using Spravy.Service.Extensions;
using Spravy.Service.HostedServices;
using Spravy.Service.Services;

namespace Spravy.Schedule.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSchedule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<MigratorHostedService<SpravyScheduleDbContext>>();
        serviceCollection.AddHostedService<ScheduleHostedService>();

        serviceCollection.AddSingleton<IFactory<string, SpravyScheduleDbContext>, SpravyScheduleDbContextFactory>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteScheduleDbContextSetup>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<GrpcEventBusServiceOptions>());
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<GrpcAuthenticationServiceOptions>());
        serviceCollection.AddSingleton<IFactory<string, IEventBusService>, EventBusServiceFactory>();
        serviceCollection.AddSingleton<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();
        serviceCollection.AddSingleton<IAuthenticationService, GrpcAuthenticationService>();
        serviceCollection.AddSingleton<ITokenService, TokenService>();
        serviceCollection.AddSingleton<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<TokenHttpHeaderFactory>();
        serviceCollection.AddSingleton<ContextAccessorHttpHeaderFactory>();
        serviceCollection.AddSingleton<IHttpHeaderFactory, TokenHttpHeaderFactory>();

        serviceCollection.AddTransient<IScheduleService, EfScheduleService>();

        serviceCollection.AddMapperConfiguration<SpravyScheduleProfile, SpravyScheduleDbProfile>();
        serviceCollection.AddSpravySqliteFolderContext<SpravyScheduleDbContext>();

        serviceCollection.AddSingleton<ITokenService>(
            sp =>
            {
                var tokenService = new TokenService(sp.GetRequiredService<IAuthenticationService>());
                var refreshToken = sp.GetRequiredService<GrpcEventBusServiceOptions>().Token.ThrowIfNullOrWhiteSpace();
                tokenService.Login(new TokenResult(refreshToken, refreshToken));

                return tokenService;
            }
        );

        return serviceCollection;
    }
}
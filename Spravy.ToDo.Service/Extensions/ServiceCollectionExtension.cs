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
using Spravy.EventBus.Domain.Mapper.Profiles;
using Spravy.Service.Extensions;
using Spravy.Service.HostedServices;
using Spravy.Service.Services;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Mapper.Profiles;
using Spravy.ToDo.Db.Services;
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
        serviceCollection.AddHostedService<MigratorHostedService<SpravyToDoDbContext>>();
        serviceCollection.AddHostedService<EventBusHostedService>();

        serviceCollection.AddSingleton<IDbContextSetup, SqliteToDoDbContextSetup>();
        serviceCollection.AddSingleton<IFactory<string, SpravyToDoDbContext>, SpravyToDoDbContextFactory>();
        serviceCollection.AddSingleton<IEventBusService, GrpcEventBusService>();
        serviceCollection.AddSingleton<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();
        serviceCollection.AddSingleton<ITokenService, TokenService>();
        serviceCollection.AddSingleton<IAuthenticationService, GrpcAuthenticationService>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<GrpcEventBusServiceOptions>());
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<GrpcAuthenticationServiceOptions>());
        serviceCollection.AddSingleton<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<TokenHttpHeaderFactory>();
        serviceCollection.AddSingleton<ContextAccessorHttpHeaderFactory>();
        serviceCollection.AddSingleton<IFactory<string, IEventBusService>, EventBusServiceFactory>();
        serviceCollection.AddSingleton<IHttpHeaderFactory, TokenHttpHeaderFactory>();

        serviceCollection.AddScoped<IToDoService, EfToDoService>();
        serviceCollection.AddScoped<StatusToDoItemService>();
        serviceCollection.AddScoped<ActiveToDoItemToDoItemService>();

        serviceCollection.AddSpravySqliteFolderContext<SpravyToDoDbContext>();

        serviceCollection.AddMapperConfiguration<SpravyToDoProfile, SpravyToDoDbProfile, SpravyEventBusProfile>();

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
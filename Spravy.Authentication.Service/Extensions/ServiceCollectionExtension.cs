using System.IdentityModel.Tokens.Jwt;
using Spravy.Authentication.Db.Contexts;
using Spravy.Authentication.Db.Mapper.Profiles;
using Spravy.Authentication.Db.Sqlite.Migrator;
using Spravy.Authentication.Db.Sqlite.Services;
using Spravy.Authentication.Domain.Core.Profiles;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Service.Helpers;
using Spravy.Authentication.Service.HostedServices;
using Spravy.Authentication.Service.Interfaces;
using Spravy.Authentication.Service.Models;
using Spravy.Authentication.Service.Services;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Extensions;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Client.Services;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.Service.Extensions;
using ISpravyAuthenticationDbContextFactory =
    Spravy.Domain.Interfaces.IFactory<string, Spravy.Authentication.Db.Contexts.SpravyAuthenticationDbContext>;

namespace Spravy.Authentication.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterAuthentication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMapperConfiguration<SpravyAuthenticationProfile, SpravyAuthenticationDbProfile>();
        serviceCollection.AddScoped<IAuthenticationService, EfAuthenticationService>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteAuthenticationDbContextSetup>();
        serviceCollection.AddScoped<IHasher, Hasher>();
        serviceCollection.AddScoped<IFactory<string, IHasher>, HasherFactory>();
        serviceCollection.AddScoped<IFactory<string, Named<IBytesToString>>, BytesToStringFactory>();
        serviceCollection.AddScoped<IFactory<string, Named<IStringToBytes>>, StringToBytesFactory>();
        serviceCollection.AddScoped<IFactory<string, Named<IHashService>>, HashServiceFactory>();
        serviceCollection.AddScoped<ITokenFactory, JwtTokenFactory>();
        serviceCollection.AddScoped<JwtSecurityTokenHandler>();
        serviceCollection.AddSingleton<IEventBusService, GrpcEventBusService>();
        serviceCollection.AddSingleton<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<GrpcEventBusServiceOptions>());
        serviceCollection.AddScoped(sp => sp.GetConfigurationSection<JwtTokenFactoryOptions>());
        serviceCollection.AddScoped(_ => NamedHelper.BytesToUpperCaseHexString.ToRef());
        serviceCollection.AddScoped(_ => NamedHelper.Sha512Hash.ToRef());
        serviceCollection.AddScoped(_ => NamedHelper.StringToUtf8Bytes.ToRef());
        serviceCollection.AddSpravyDbContext<SpravyAuthenticationDbContext>();
        serviceCollection.AddHostedService<MigratorHostedService>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFileOptions>());
        serviceCollection.AddSingleton<ISpravyAuthenticationDbContextFactory, SpravyAuthenticationDbContextFactory>();

        return serviceCollection;
    }
}
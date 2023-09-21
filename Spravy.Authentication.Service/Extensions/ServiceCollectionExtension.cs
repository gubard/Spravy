using System.IdentityModel.Tokens.Jwt;
using Spravy.Authentication.Db.Contexts;
using Spravy.Authentication.Db.Mapper.Profiles;
using Spravy.Authentication.Db.Sqlite.Migrator;
using Spravy.Authentication.Db.Sqlite.Services;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Mapper.Profiles;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Domain.Services;
using Spravy.Authentication.Service.Helpers;
using Spravy.Authentication.Service.HostedServices;
using Spravy.Authentication.Service.Interfaces;
using Spravy.Authentication.Service.Models;
using Spravy.Authentication.Service.Services;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
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
using Spravy.Service.Services;
using ISpravyAuthenticationDbContextFactory =
    Spravy.Domain.Interfaces.IFactory<string, Spravy.Authentication.Db.Contexts.SpravyAuthenticationDbContext>;
using TokenService = Spravy.Authentication.Service.Services.TokenService;

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
        serviceCollection.AddSingleton<ITokenFactory, JwtTokenFactory>();
        serviceCollection.AddSingleton<JwtSecurityTokenHandler>();
        serviceCollection.AddSingleton<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<GrpcEventBusServiceOptions>());
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<JwtTokenFactoryOptions>());
        serviceCollection.AddScoped(_ => NamedHelper.BytesToUpperCaseHexString.ToRef());
        serviceCollection.AddScoped(_ => NamedHelper.Sha512Hash.ToRef());
        serviceCollection.AddScoped(_ => NamedHelper.StringToUtf8Bytes.ToRef());
        serviceCollection.AddSpravySqliteFileDbContext<SpravyAuthenticationDbContext>();
        serviceCollection.AddHostedService<MigratorHostedService>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFileOptions>());
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFileOptions>());
        serviceCollection.AddSingleton<ISpravyAuthenticationDbContextFactory, SpravyAuthenticationDbContextFactory>();
        serviceCollection.AddSingleton<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<TokenHttpHeaderFactory>();
        serviceCollection.AddSingleton<ContextAccessorHttpHeaderFactory>();
        serviceCollection.AddSingleton<IEventBusService, GrpcEventBusService>();

        serviceCollection.AddSingleton<ITokenService>(
            sp =>
            {
                var tokenService = new TokenService(sp.GetRequiredService<ITokenFactory>());
                var refreshToken = sp.GetRequiredService<GrpcEventBusServiceOptions>().Token.ThrowIfNullOrWhiteSpace();
                tokenService.Login(new TokenResult(refreshToken, refreshToken));

                return tokenService;
            }
        );

        serviceCollection.AddSingleton<IHttpHeaderFactory, CombineHttpHeaderFactory>(
            sp => new CombineHttpHeaderFactory(
                new IHttpHeaderFactory[]
                {
                    sp.GetRequiredService<TokenHttpHeaderFactory>(),
                    sp.GetRequiredService<ContextAccessorHttpHeaderFactory>(),
                }
            )
        );

        return serviceCollection;
    }
}
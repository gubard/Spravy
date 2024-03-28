using System.IdentityModel.Tokens.Jwt;
using Spravy.Authentication.Db.Contexts;
using Spravy.Authentication.Db.Mapper.Profiles;
using Spravy.Authentication.Db.Sqlite.Migrator;
using Spravy.Authentication.Db.Sqlite.Services;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Mapper.Profiles;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Service.Helpers;
using Spravy.Authentication.Service.Interfaces;
using Spravy.Authentication.Service.Models;
using Spravy.Authentication.Service.Services;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Extensions;
using Spravy.Db.Sqlite.Models;
using Spravy.Di.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Client.Services;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.Service.Extensions;
using Spravy.Service.Services;
using Spravy.Client.Extensions;
using Spravy.Core.Options;
using Spravy.Core.Services;
using Spravy.EventBus.Protos;
using Spravy.Service.HostedServices;

namespace Spravy.Authentication.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterAuthentication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<FileMigratorHostedService<SpravyDbAuthenticationDbContext>>();
        serviceCollection.AddMapperConfiguration<SpravyAuthenticationProfile, SpravyAuthenticationDbProfile>();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteAuthenticationDbContextSetup>();
        serviceCollection.AddSingleton<ITokenFactory, JwtTokenFactory>();
        serviceCollection.AddSingleton<ISerializer, ProtobufSerializer>();
        serviceCollection.AddSingleton<JwtSecurityTokenHandler>();
        serviceCollection.AddSingleton<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<JwtTokenFactoryOptions>());
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFileOptions>());
        serviceCollection.AddSingleton<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<ContextAccessorUserIdHttpHeaderFactory>();
        serviceCollection.AddSingleton<IEventBusService>(sp => sp.GetRequiredService<GrpcEventBusService>());
        serviceCollection.AddSingleton<ITokenService, TokenService>();
        serviceCollection.AddTransient<IFactory<string, IHasher>, HasherFactory>();
        serviceCollection.AddTransient<IFactory<string, Named<IBytesToString>>, BytesToStringFactory>();
        serviceCollection.AddTransient<IFactory<string, Named<IStringToBytes>>, StringToBytesFactory>();
        serviceCollection.AddTransient<IFactory<string, Named<IHashService>>, HashServiceFactory>();
        serviceCollection.AddTransient(_ => NamedHelper.BytesToUpperCaseHexString.ToRef());
        serviceCollection.AddTransient(_ => NamedHelper.Sha512Hash.ToRef());
        serviceCollection.AddTransient(_ => NamedHelper.StringToUtf8Bytes.ToRef());
        serviceCollection.AddSingleton<TimeZoneHttpHeaderFactory>();
        serviceCollection.AddTransient<IEmailService, EmailService>();
        serviceCollection.AddTransient<IRandom<string>>(
            _ => new RandomString("QAZWSXEDCRFVTGBYHNUJMIKOP0123456789", 6)
        );
        serviceCollection.AddTransient(sp => sp.GetConfigurationSection<EmailOptions>());
        serviceCollection.AddTransient<IPasswordValidator>(_ => PasswordValidator.Default);
        serviceCollection.AddTransient<ILoginValidator>(_ => LoginValidator.Default);
        serviceCollection.AddTransient<IAuthenticationService, EfAuthenticationService>();
        serviceCollection.AddTransient<IHasher, Hasher>();

        serviceCollection.AddSpravySqliteFileDbContext<SpravyDbAuthenticationDbContext,
            SpravyAuthenticationDbSqliteMigratorMark>();

        serviceCollection.AddGrpcServiceAuth<GrpcEventBusService,
            EventBusService.EventBusServiceClient,
            GrpcEventBusServiceOptions>();

        serviceCollection.AddTransient<IFactory<string,
                SpravyDbAuthenticationDbContext>,
            SpravyAuthenticationDbContextFactory>();

        serviceCollection.AddTransient<IHttpHeaderFactory>(
            sp => new CombineHttpHeaderFactory(
                sp.GetRequiredService<ContextAccessorUserIdHttpHeaderFactory>(),
                sp.GetRequiredService<TimeZoneHttpHeaderFactory>()
            )
        );

        return serviceCollection;
    }
}
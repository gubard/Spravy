using TokenService = Spravy.Authentication.Service.Services.TokenService;

namespace Spravy.Authentication.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterAuthentication(
        this IServiceCollection serviceCollection
    )
    {
        serviceCollection.AddTransient<IRetryService, RetryService>();
        serviceCollection.AddHostedService<
            FileMigratorHostedService<SpravyDbAuthenticationDbContext>
        >();
        serviceCollection.AddSingleton<IDbContextSetup, SqliteAuthenticationDbContextSetup>();
        serviceCollection.AddSingleton<ITokenFactory, JwtTokenFactory>();
        serviceCollection.AddSingleton<JwtSecurityTokenHandler>();
        serviceCollection.AddSingleton<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<JwtTokenFactoryOptions>());
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFileOptions>());
        serviceCollection.AddSingleton<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<ContextAccessorUserIdHttpHeaderFactory>();
        serviceCollection.AddTransient<IEventBusService>(sp =>
            sp.GetRequiredService<GrpcEventBusService>()
        );
        serviceCollection.AddSingleton<ITokenService, TokenService>();
        serviceCollection.AddTransient<IRpcExceptionHandler, RpcExceptionHandler>();
        serviceCollection.AddTransient<IFactory<string, IHasher>, HasherFactory>();
        serviceCollection.AddTransient<
            IFactory<string, Named<IBytesToString>>,
            BytesToStringFactory
        >();
        serviceCollection.AddTransient<
            IFactory<string, Named<IStringToBytes>>,
            StringToBytesFactory
        >();
        serviceCollection.AddTransient<IFactory<string, Named<IHashService>>, HashServiceFactory>();
        serviceCollection.AddTransient(_ => NamedHelper.BytesToUpperCaseHexString.ToRef());
        serviceCollection.AddTransient(_ => NamedHelper.Sha512Hash.ToRef());
        serviceCollection.AddTransient(_ => NamedHelper.StringToUtf8Bytes.ToRef());
        serviceCollection.AddSingleton<TimeZoneHttpHeaderFactory>();
        serviceCollection.AddTransient<IEmailService, EmailService>();
        serviceCollection.AddTransient<ISerializer, SpravyJsonSerializer>();
        serviceCollection.AddTransient<JsonSerializerContext, SpravyJsonSerializerContext>();

        serviceCollection.AddTransient<IRandom<string>>(_ => new RandomString(
            "QAZWSXEDCRFVTGBYHNUJMIKOP0123456789",
            6
        ));

        serviceCollection.AddTransient(sp => sp.GetConfigurationSection<EmailOptions>());
        serviceCollection.AddTransient<IPasswordValidator>(_ => PasswordValidator.Default);
        serviceCollection.AddTransient<ILoginValidator>(_ => LoginValidator.Default);
        serviceCollection.AddTransient<IAuthenticationService, EfAuthenticationService>();
        serviceCollection.AddTransient<IHasher, Hasher>();

        serviceCollection.AddSpravySqliteFileDbContext<
            SpravyDbAuthenticationDbContext,
            SpravyAuthenticationDbSqliteMigratorMark
        >();

        serviceCollection.AddGrpcServiceAuth<
            GrpcEventBusService,
            EventBusService.EventBusServiceClient,
            GrpcEventBusServiceOptions
        >();

        serviceCollection.AddTransient<
            IFactory<string, SpravyDbAuthenticationDbContext>,
            SpravyAuthenticationDbContextFactory
        >();

        serviceCollection.AddTransient<IHttpHeaderFactory>(sp => new CombineHttpHeaderFactory(
            sp.GetRequiredService<ContextAccessorUserIdHttpHeaderFactory>(),
            sp.GetRequiredService<TimeZoneHttpHeaderFactory>()
        ));

        return serviceCollection;
    }
}

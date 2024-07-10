using Spravy.Core.Extensions;
using Spravy.Core.Interfaces;

namespace Spravy.Client.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddGrpcService<TGrpcService, TGrpcClient, TGrpcOptions>(
        this IServiceCollection serviceCollection
    )
        where TGrpcService : GrpcServiceBase<TGrpcClient>,
            IGrpcServiceCreator<TGrpcService, TGrpcClient>
        where TGrpcClient : ClientBase
        where TGrpcOptions : class, IGrpcOptionsValue
    {
        serviceCollection.AddTransient<
            ICacheValidator<Uri, GrpcChannel>,
            GrpcChannelCacheValidator
        >();

        serviceCollection.AddTransient(sp => sp.GetConfigurationSection<TGrpcOptions>());

        serviceCollection.AddTransient<IFactory<Uri, TGrpcClient>>(sp =>
        {
            var options = sp.GetConfigurationSection<TGrpcOptions>();

            var grpcChannelCacheFactory = new CacheFactory<Uri, GrpcChannel>(
                new GrpcChannelFactory(
                    options.ChannelType,
                    options.ChannelCredentialType.GetChannelCredentials()
                ),
                sp.GetRequiredService<ICacheValidator<Uri, GrpcChannel>>()
            );

            return new CacheFactory<Uri, TGrpcClient>(
                new GrpcClientFactory<TGrpcClient>(grpcChannelCacheFactory),
                new GrpcClientCacheValidator<TGrpcClient>(
                    sp.GetRequiredService<ICacheValidator<Uri, GrpcChannel>>(),
                    grpcChannelCacheFactory
                )
            );
        });

        serviceCollection.AddTransient<TGrpcService>(sp =>
        {
            var options = sp.GetConfigurationSection<TGrpcOptions>();

            return TGrpcService.CreateGrpcService(
                sp.GetRequiredService<IFactory<Uri, TGrpcClient>>(),
                options.Host.ThrowIfNull().ToUri(),
                sp.GetRequiredService<IRpcExceptionHandler>()
            );
        });

        return serviceCollection;
    }

    public static IServiceCollection AddGrpcServiceAuth<TGrpcService, TGrpcClient, TGrpcOptions>(
        this IServiceCollection serviceCollection
    )
        where TGrpcService : GrpcServiceBase<TGrpcClient>,
            IGrpcServiceCreatorAuth<TGrpcService, TGrpcClient>
        where TGrpcClient : ClientBase
        where TGrpcOptions : class, IGrpcOptionsValue
    {
        serviceCollection.AddTransient<
            ICacheValidator<Uri, GrpcChannel>,
            GrpcChannelCacheValidator
        >();

        serviceCollection.AddTransient(sp => sp.GetConfigurationSection<TGrpcOptions>());

        serviceCollection.AddTransient<IFactory<Uri, TGrpcClient>>(sp =>
        {
            var options = sp.GetConfigurationSection<TGrpcOptions>();

            var grpcChannelCacheFactory = new CacheFactory<Uri, GrpcChannel>(
                new GrpcChannelFactory(
                    options.ChannelType,
                    options.ChannelCredentialType.GetChannelCredentials()
                ),
                sp.GetRequiredService<ICacheValidator<Uri, GrpcChannel>>()
            );

            return new CacheFactory<Uri, TGrpcClient>(
                new GrpcClientFactory<TGrpcClient>(grpcChannelCacheFactory),
                new GrpcClientCacheValidator<TGrpcClient>(
                    sp.GetRequiredService<ICacheValidator<Uri, GrpcChannel>>(),
                    grpcChannelCacheFactory
                )
            );
        });

        serviceCollection.AddTransient<TGrpcService>(sp =>
        {
            var options = sp.GetConfigurationSection<TGrpcOptions>();

            return TGrpcService.CreateGrpcService(
                sp.GetRequiredService<IFactory<Uri, TGrpcClient>>(),
                options.Host.ThrowIfNull().ToUri(),
                CreateMetadataFactory(options, sp),
                sp.GetRequiredService<IRpcExceptionHandler>()
            );
        });

        return serviceCollection;
    }

    private static IMetadataFactory CreateMetadataFactory<TGrpcOptions>(
        TGrpcOptions options,
        IServiceProvider serviceProvider
    )
        where TGrpcOptions : IGrpcOptionsValue
    {
        if (options.Token.IsNullOrWhiteSpace())
        {
            return serviceProvider.GetRequiredService<IMetadataFactory>();
        }

        var tokenService = serviceProvider.GetRequiredService<ITokenService>();
        tokenService.LoginAsync(options.Token, CancellationToken.None).GetAwaiter().GetResult();
        var tokenHttpHeaderFactory = new TokenHttpHeaderFactory(tokenService);

        var combineHttpHeaderFactory = new CombineHttpHeaderFactory(
            serviceProvider.GetRequiredService<IHttpHeaderFactory>(),
            tokenHttpHeaderFactory
        );

        return new MetadataFactory(combineHttpHeaderFactory);
    }
}

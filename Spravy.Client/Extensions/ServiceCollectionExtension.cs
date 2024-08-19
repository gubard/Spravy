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
            var clientFactory = sp.GetRequiredService<IFactory<ChannelBase, TGrpcClient>>();

            var grpcChannelCacheFactory = new CacheFactory<Uri, GrpcChannel>(
                new GrpcChannelFactory(
                    options.ChannelType,
                    options.ChannelCredentialType.GetChannelCredentials()
                ),
                sp.GetRequiredService<ICacheValidator<Uri, GrpcChannel>>()
            );

            return new CacheFactory<Uri, TGrpcClient>(
                new GrpcClientFactory<TGrpcClient>(grpcChannelCacheFactory, clientFactory),
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
                sp.GetRequiredService<IRpcExceptionHandler>(),
                sp.GetRequiredService<IRetryService>()
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
            var clientFactory = sp.GetRequiredService<IFactory<ChannelBase, TGrpcClient>>();

            var grpcChannelCacheFactory = new CacheFactory<Uri, GrpcChannel>(
                new GrpcChannelFactory(
                    options.ChannelType,
                    options.ChannelCredentialType.GetChannelCredentials()
                ),
                sp.GetRequiredService<ICacheValidator<Uri, GrpcChannel>>()
            );

            return new CacheFactory<Uri, TGrpcClient>(
                new GrpcClientFactory<TGrpcClient>(grpcChannelCacheFactory, clientFactory),
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
                sp.GetRequiredService<IMetadataFactory>(),
                sp.GetRequiredService<IRpcExceptionHandler>(),
                sp.GetRequiredService<IRetryService>()
            );
        });

        return serviceCollection;
    }
}

using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using Ninject.Modules;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Di.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;

namespace Spravy.Domain.Di.Extensions;

public static class NinjectModuleExtension
{
    public static NinjectModule BindGrpcService2<TGrpcService, TGrpcClient, TGrpcOptions>(
        this NinjectModule module
    )
        where TGrpcService : GrpcServiceBase<TGrpcClient>, IGrpcServiceCreator2<TGrpcService, TGrpcClient>
        where TGrpcClient : ClientBase
        where TGrpcOptions : class, IGrpcOptionsValue
    {
        module.Bind<TGrpcOptions>().ToMethod(context => context.Kernel.GetConfigurationSection<TGrpcOptions>());

        module.Bind<IFactory<Uri, TGrpcClient>>()
            .ToMethod(
                context =>
                {
                    var options = context.Kernel.GetConfigurationSection<TGrpcOptions>();

                    var grpcChannelCacheFactory = new CacheFactory<Uri, GrpcChannel>(
                        new GrpcChannelFactory(
                            options.ChannelType,
                            options.ChannelCredentialType.GetChannelCredentials()
                        ),
                        context.Kernel.GetRequiredService<ICacheValidator<Uri, GrpcChannel>>()
                    );

                    return new CacheFactory<Uri, TGrpcClient>(
                        new GrpcClientFactory<TGrpcClient>(grpcChannelCacheFactory),
                        new GrpcClientCacheValidator<TGrpcClient>(
                            context.Kernel.GetRequiredService<ICacheValidator<Uri, GrpcChannel>>(),
                            grpcChannelCacheFactory
                        )
                    );
                }
            );

        module.Bind<TGrpcService>()
            .ToMethod(
                context =>
                {
                    var options = context.Kernel.GetConfigurationSection<TGrpcOptions>();
                    var grpcClientFactory = context.Kernel.GetRequiredService<IFactory<Uri, TGrpcClient>>();
                    var host = options.Host.ThrowIfNull().ToUri();
                    var mapper = context.Kernel.GetRequiredService<IMapper>();
                    var grpcService = TGrpcService.CreateGrpcService(grpcClientFactory, host, mapper);

                    return grpcService;
                }
            );

        return module;
    }

    public static NinjectModule BindGrpcService<TGrpcService, TGrpcClient, TGrpcOptions>(
        this NinjectModule module
    )
        where TGrpcService : GrpcServiceBase<TGrpcClient>, IGrpcServiceCreator<TGrpcService, TGrpcClient>
        where TGrpcClient : ClientBase
        where TGrpcOptions : class, IGrpcOptionsValue
    {
        module.Bind<TGrpcOptions>().ToMethod(context => context.Kernel.GetConfigurationSection<TGrpcOptions>());

        module.Bind<IFactory<Uri, TGrpcClient>>()
            .ToMethod(
                context =>
                {
                    var options = context.Kernel.GetConfigurationSection<TGrpcOptions>();

                    var grpcChannelCacheFactory = new CacheFactory<Uri, GrpcChannel>(
                        new GrpcChannelFactory(
                            options.ChannelType,
                            options.ChannelCredentialType.GetChannelCredentials()
                        ),
                        context.Kernel.GetRequiredService<ICacheValidator<Uri, GrpcChannel>>()
                    );

                    return new CacheFactory<Uri, TGrpcClient>(
                        new GrpcClientFactory<TGrpcClient>(grpcChannelCacheFactory),
                        new GrpcClientCacheValidator<TGrpcClient>(
                            context.Kernel.GetRequiredService<ICacheValidator<Uri, GrpcChannel>>(),
                            grpcChannelCacheFactory
                        )
                    );
                }
            );

        module.Bind<TGrpcService>()
            .ToMethod(
                context =>
                {
                    var options = context.Kernel.GetConfigurationSection<TGrpcOptions>();

                    return TGrpcService.CreateGrpcService(
                        context.Kernel.GetRequiredService<IFactory<Uri, TGrpcClient>>(),
                        options.Host.ThrowIfNull().ToUri(),
                        context.Kernel.GetRequiredService<IMapper>(),
                        CreateMetadataFactory(options, context.Kernel)
                    );
                }
            );

        return module;
    }

    private static IMetadataFactory CreateMetadataFactory<TGrpcOptions>(
        TGrpcOptions options,
        IKernel kernel
    ) where TGrpcOptions : IGrpcOptionsValue
    {
        if (options.Token.IsNullOrWhiteSpace())
        {
            return kernel.GetRequiredService<IMetadataFactory>();
        }

        var tokenService = kernel.GetRequiredService<ITokenService>();
        tokenService.LoginAsync(options.Token).GetAwaiter().GetResult();
        var tokenHttpHeaderFactory = new TokenHttpHeaderFactory(tokenService);

        var combineHttpHeaderFactory = new CombineHttpHeaderFactory(
            kernel.GetRequiredService<IHttpHeaderFactory>(),
            tokenHttpHeaderFactory
        );

        return new MetadataFactory(combineHttpHeaderFactory);
    }
}
using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using Ninject.Modules;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;

namespace Spravy.Domain.Di.Extensions;

public static class NinjectModuleExtension
{
    public static NinjectModule BindGrpcService2<TGrpcService, TGrpcClient, TGrpcOptions>(
        this NinjectModule module,
        bool useCache = true
    )
        where TGrpcService : GrpcServiceBase<TGrpcClient>, IGrpcServiceCreator2<TGrpcService, TGrpcClient>
        where TGrpcClient : ClientBase
        where TGrpcOptions : class, IGrpcOptionsValue
    {
        if (useCache)
        {
            module.Bind<IFactory<Uri, TGrpcClient>>()
                .ToMethod(
                    context =>
                    {
                        var options = context.Kernel.GetRequiredService<TGrpcOptions>();
                        var channelCredentials = options.ChannelCredentialType.GetChannelCredentials();
                        var grpcChannelFactory = new GrpcChannelFactory(options.ChannelType, channelCredentials);
                        var cacheValidator = context.Kernel.GetRequiredService<ICacheValidator<Uri, GrpcChannel>>();
                        var cacheFactory = new CacheFactory<Uri, GrpcChannel>(grpcChannelFactory, cacheValidator);
                        var clientFactory = new GrpcClientFactory<TGrpcClient>(cacheFactory);
                        var clientValidator = new GrpcClientCacheValidator<TGrpcClient>(cacheValidator, cacheFactory);

                        return new CacheFactory<Uri, TGrpcClient>(clientFactory, clientValidator);
                    }
                );
        }
        else
        {
            module.Bind<IFactory<Uri, TGrpcClient>>()
                .ToMethod(
                    context =>
                    {
                        var options = context.Kernel.GetRequiredService<TGrpcOptions>();
                        var channelCredentials = options.ChannelCredentialType.GetChannelCredentials();
                        var channelFactory = new GrpcChannelFactory(options.ChannelType, channelCredentials);

                        return new GrpcClientFactory<TGrpcClient>(channelFactory);
                    }
                );
        }

        module.Bind<TGrpcService>()
            .ToMethod(
                context =>
                {
                    var options = context.Kernel.GetRequiredService<TGrpcOptions>();
                    var grpcClientFactory = context.Kernel.GetRequiredService<IFactory<Uri, TGrpcClient>>();
                    var host = options.Host.ThrowIfNull().ToUri();
                    var mapper = context.Kernel.GetRequiredService<IMapper>();

                    return TGrpcService.CreateGrpcService(grpcClientFactory, host, mapper);
                }
            );

        return module;
    }

    public static NinjectModule BindGrpcService<TGrpcService, TGrpcClient, TGrpcOptions>(
        this NinjectModule module,
        bool useCache = true
    )
        where TGrpcService : GrpcServiceBase<TGrpcClient>, IGrpcServiceCreator<TGrpcService, TGrpcClient>
        where TGrpcClient : ClientBase
        where TGrpcOptions : class, IGrpcOptionsValue
    {
        if (useCache)
        {
            module.Bind<IFactory<Uri, TGrpcClient>>()
                .ToMethod(
                    context =>
                    {
                        var options = context.Kernel.GetRequiredService<TGrpcOptions>();
                        var channelCredentials = options.ChannelCredentialType.GetChannelCredentials();
                        var grpcChannelFactory = new GrpcChannelFactory(options.ChannelType, channelCredentials);
                        var cacheValidator = context.Kernel.GetRequiredService<ICacheValidator<Uri, GrpcChannel>>();
                        var cacheFactory = new CacheFactory<Uri, GrpcChannel>(grpcChannelFactory, cacheValidator);
                        var grpcClientFactory = new GrpcClientFactory<TGrpcClient>(cacheFactory);
                        var clientValidator = new GrpcClientCacheValidator<TGrpcClient>(cacheValidator, cacheFactory);

                        return new CacheFactory<Uri, TGrpcClient>(grpcClientFactory, clientValidator);
                    }
                );
        }
        else
        {
            module.Bind<IFactory<Uri, TGrpcClient>>()
                .ToMethod(
                    context =>
                    {
                        var options = context.Kernel.GetRequiredService<TGrpcOptions>();
                        var channelCredentials = options.ChannelCredentialType.GetChannelCredentials();
                        var channelFactory = new GrpcChannelFactory(options.ChannelType, channelCredentials);

                        return new GrpcClientFactory<TGrpcClient>(channelFactory);
                    }
                );
        }

        module.Bind<TGrpcService>()
            .ToMethod(
                context =>
                {
                    var options = context.Kernel.GetRequiredService<TGrpcOptions>();
                    var factory = context.Kernel.GetRequiredService<IFactory<Uri, TGrpcClient>>();
                    var mapper = context.Kernel.GetRequiredService<IMapper>();
                    var host = options.Host.ThrowIfNull().ToUri();
                    var metadataFactory = CreateMetadataFactory(options, context.Kernel);

                    return TGrpcService.CreateGrpcService(factory, host, mapper, metadataFactory);
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
        var httpHeaderFactory = kernel.GetRequiredService<IHttpHeaderFactory>();
        var combineHttpHeaderFactory = new CombineHttpHeaderFactory(httpHeaderFactory, tokenHttpHeaderFactory);

        return new MetadataFactory(combineHttpHeaderFactory);
    }
}
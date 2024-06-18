namespace Spravy.Client.Helpers;

public static class GrpcClientFactoryHelper
{
    public static IFactory<Uri, TGrpcClient> CreateCacheGrpcFactory<TGrpcService, TGrpcClient, TGrpcOptions>(
        TGrpcOptions options,
        ICacheValidator<Uri, GrpcChannel> cacheValidator
    )
        where TGrpcService : GrpcServiceBase<TGrpcClient>
        where TGrpcClient : ClientBase
        where TGrpcOptions : class, IGrpcOptionsValue
    {
        var channelCredentials = options.ChannelCredentialType.GetChannelCredentials();
        var grpcChannelFactory = new GrpcChannelFactory(options.ChannelType, channelCredentials);
        var cacheFactory = new CacheFactory<Uri, GrpcChannel>(grpcChannelFactory, cacheValidator);
        var clientFactory = new GrpcClientFactory<TGrpcClient>(cacheFactory);
        var clientValidator = new GrpcClientCacheValidator<TGrpcClient>(cacheValidator, cacheFactory);
        
        return new CacheFactory<Uri, TGrpcClient>(clientFactory, clientValidator);
    }
    
    public static IFactory<Uri, TGrpcClient>
        CreateGrpcFactory<TGrpcService, TGrpcClient, TGrpcOptions>(TGrpcOptions options)
        where TGrpcService : GrpcServiceBase<TGrpcClient>
        where TGrpcClient : ClientBase
        where TGrpcOptions : class, IGrpcOptionsValue
    {
        var channelCredentials = options.ChannelCredentialType.GetChannelCredentials();
        var channelFactory = new GrpcChannelFactory(options.ChannelType, channelCredentials);
        
        return new GrpcClientFactory<TGrpcClient>(channelFactory);
    }
    
    public static TGrpcService CreateGrpcService<TGrpcService, TGrpcClient, TGrpcOptions>(
        TGrpcOptions options,
        IFactory<Uri, TGrpcClient> grpcClientFactory,
        ISerializer serializer
    )
        where TGrpcService : GrpcServiceBase<TGrpcClient>, IGrpcServiceCreator<TGrpcService, TGrpcClient>
        where TGrpcClient : ClientBase
        where TGrpcOptions : class, IGrpcOptionsValue
    {
        var host = options.Host.ThrowIfNullOrWhiteSpace().ToUri();
        
        return TGrpcService.CreateGrpcService(grpcClientFactory, host, serializer);
    }
    
    public static TGrpcService CreateGrpcServiceAuth<TGrpcService, TGrpcClient, TGrpcOptions>(
        TGrpcOptions options,
        IFactory<Uri, TGrpcClient> grpcClientFactory,
        ISerializer serializer,
        IMetadataFactory metadataFactory
    )
        where TGrpcService : GrpcServiceBase<TGrpcClient>, IGrpcServiceCreatorAuth<TGrpcService, TGrpcClient>
        where TGrpcClient : ClientBase
        where TGrpcOptions : class, IGrpcOptionsValue
    {
        var host = options.Host.ThrowIfNullOrWhiteSpace().ToUri();
        
        return TGrpcService.CreateGrpcService(grpcClientFactory, host, metadataFactory, serializer);
    }
    
    private static IMetadataFactory CreateMetadataFactory<TGrpcOptions>(
        TGrpcOptions options,
        ITokenService tokenService,
        IHttpHeaderFactory httpHeaderFactory
    ) where TGrpcOptions : IGrpcOptionsValue
    {
        tokenService.LoginAsync(options.Token.ThrowIfNullOrWhiteSpace(), CancellationToken.None)
           .GetAwaiter()
           .GetResult();
        
        var tokenHttpHeaderFactory = new TokenHttpHeaderFactory(tokenService);
        var combineHttpHeaderFactory = new CombineHttpHeaderFactory(httpHeaderFactory, tokenHttpHeaderFactory);
        
        return new MetadataFactory(combineHttpHeaderFactory);
    }
}
namespace Spravy.ToDo.Domain.Client.Modules;

[ServiceProviderModule]
[Singleton(typeof(GrpcToDoServiceOptions), Factory = nameof(GrpcToDoServiceOptionsFactory))]
[Singleton(
    typeof(IFactory<Uri, ToDoService.ToDoServiceClient>),
    Factory = nameof(ToDoServiceClientsFactory)
)]
[Transient(typeof(IToDoService), Factory = nameof(ToDoServiceFactory))]
public interface IToDoClientModule
{
    static IToDoClientModule()
    {
        GrpcClientFactoryHelper.ClientFactories.Add(
            typeof(ToDoService.ToDoServiceClient),
            new ToDoServiceClientFactory()
        );
    }

    static GrpcToDoServiceOptions GrpcToDoServiceOptionsFactory(
        ISerializer serializer,
        IConfigurationLoader configurationLoader
    )
    {
        using var stream = configurationLoader.GetStream();
        var configuration = serializer.Deserialize<GrpcToDoServiceOptionsConfiguration>(stream);

        return configuration.ThrowIfError().GrpcToDoService.ThrowIfNull();
    }

    static IFactory<Uri, ToDoService.ToDoServiceClient> ToDoServiceClientsFactory(
        ClientOptions options,
        GrpcToDoServiceOptions serviceOptions,
        ICacheValidator<Uri, GrpcChannel> cacheValidator
    )
    {
        if (options.UseCache)
        {
            return GrpcClientFactoryHelper.CreateCacheGrpcFactory<
                GrpcToDoService,
                ToDoService.ToDoServiceClient,
                GrpcToDoServiceOptions
            >(serviceOptions, cacheValidator);
        }

        return GrpcClientFactoryHelper.CreateGrpcFactory<
            GrpcToDoService,
            ToDoService.ToDoServiceClient,
            GrpcToDoServiceOptions
        >(serviceOptions);
    }

    static IToDoService ToDoServiceFactory(
        GrpcToDoServiceOptions options,
        IFactory<Uri, ToDoService.ToDoServiceClient> grpcClientFactory,
        IRpcExceptionHandler handler,
        IMetadataFactory metadataFactory,
        IRetryService retryService
    )
    {
        return GrpcClientFactoryHelper.CreateGrpcServiceAuth<
            GrpcToDoService,
            ToDoService.ToDoServiceClient,
            GrpcToDoServiceOptions
        >(options, grpcClientFactory, handler, metadataFactory, retryService);
    }
}

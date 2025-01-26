using Grpc.Net.Client;
using Jab;
using Spravy.Client.Helpers;
using Spravy.Client.Interfaces;
using Spravy.Client.Models;
using Spravy.Core.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Picture.Domain.Client.Models;
using Spravy.Picture.Domain.Client.Services;
using Spravy.Picture.Domain.Interfaces;
using Spravy.Picture.Protos;

namespace Spravy.Picture.Domain.Client.Modules;

[ServiceProviderModule]
[Singleton(typeof(GrpcPictureServiceOptions), Factory = nameof(GrpcToDoServiceOptionsFactory))]
[Singleton(typeof(IFactory<Uri, PictureService.PictureServiceClient>), Factory = nameof(ToDoServiceClientsFactory))]
[Transient(typeof(IPictureService), Factory = nameof(ToDoServiceFactory))]
public interface IPictureClientModule
{
    static IPictureClientModule()
    {
        GrpcClientFactoryHelper.ClientFactories.Add(
            typeof(PictureService.PictureServiceClient),
            new PictureServiceClientFactory()
        );
    }

    static GrpcPictureServiceOptions GrpcToDoServiceOptionsFactory(
        ISerializer serializer,
        IConfigurationLoader configurationLoader
    )
    {
        using var stream = configurationLoader.GetStream();
        var configuration = serializer.Deserialize<GrpcPictureServiceOptionsConfiguration>(stream);

        return configuration.ThrowIfError().GrpcPictureService.ThrowIfNull();
    }

    static IFactory<Uri, PictureService.PictureServiceClient> ToDoServiceClientsFactory(
        ClientOptions options,
        GrpcPictureServiceOptions serviceOptions,
        ICacheValidator<Uri, GrpcChannel> cacheValidator
    )
    {
        if (options.UseCache)
        {
            return GrpcClientFactoryHelper
               .CreateCacheGrpcFactory<GrpcPictureService, PictureService.PictureServiceClient, GrpcPictureServiceOptions>(
                    serviceOptions,
                    cacheValidator
                );
        }

        return GrpcClientFactoryHelper
           .CreateGrpcFactory<GrpcPictureService, PictureService.PictureServiceClient, GrpcPictureServiceOptions>(serviceOptions);
    }

    static IPictureService ToDoServiceFactory(
        GrpcPictureServiceOptions options,
        IFactory<Uri, PictureService.PictureServiceClient> grpcClientFactory,
        IRpcExceptionHandler handler,
        IMetadataFactory metadataFactory,
        IRetryService retryService
    )
    {
        return GrpcClientFactoryHelper
           .CreateGrpcServiceAuth<GrpcPictureService, PictureService.PictureServiceClient, GrpcPictureServiceOptions>(
                options,
                grpcClientFactory,
                handler,
                metadataFactory,
                retryService
            );
    }
}
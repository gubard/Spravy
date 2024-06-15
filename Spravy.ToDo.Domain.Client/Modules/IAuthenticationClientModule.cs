using Grpc.Net.Client;
using Jab;
using Microsoft.Extensions.Configuration;
using Spravy.Client.Helpers;
using Spravy.Client.Interfaces;
using Spravy.Client.Models;
using Spravy.Core.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.ToDo.Domain.Client.Models;
using Spravy.ToDo.Domain.Client.Services;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Protos;

namespace Spravy.ToDo.Domain.Client.Modules;

[ServiceProviderModule]
[Singleton(typeof(GrpcToDoServiceOptions), Factory = nameof(GrpcToDoServiceOptionsFactory))]
[Singleton(typeof(IFactory<Uri, ToDoService.ToDoServiceClient>), Factory = nameof(ToDoServiceClientsFactory))]
[Transient(typeof(IToDoService), Factory = nameof(ToDoServiceFactory))]
public interface IToDoClientModule
{
    public GrpcToDoServiceOptions GrpcToDoServiceOptionsFactory(IConfiguration configuration)
    {
        return configuration.GetOptionsValue<GrpcToDoServiceOptions>();
    }
    
    public IFactory<Uri, ToDoService.ToDoServiceClient> ToDoServiceClientsFactory(
        ClientOptions options,
        GrpcToDoServiceOptions serviceOptions,
        ICacheValidator<Uri, GrpcChannel> cacheValidator
    )
    {
        if (options.UseCache)
        {
            return GrpcClientFactoryHelper
               .CreateCacheGrpcFactory<GrpcToDoService, ToDoService.ToDoServiceClient,
                    GrpcToDoServiceOptions>(serviceOptions, cacheValidator);
        }
        
        return GrpcClientFactoryHelper
           .CreateGrpcFactory<GrpcToDoService, ToDoService.ToDoServiceClient, GrpcToDoServiceOptions>(
                serviceOptions);
    }
    
    public IToDoService ToDoServiceFactory(
        GrpcToDoServiceOptions options,
        IFactory<Uri, ToDoService.ToDoServiceClient> grpcClientFactory,
        IConverter converter,
        ISerializer serializer,
        IMetadataFactory metadataFactory
    )
    {
        return GrpcClientFactoryHelper
           .CreateGrpcServiceAuth<GrpcToDoService, ToDoService.ToDoServiceClient, GrpcToDoServiceOptions>(
                options, grpcClientFactory, converter, serializer, metadataFactory);
    }
}
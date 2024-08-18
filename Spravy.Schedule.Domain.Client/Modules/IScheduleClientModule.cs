using Grpc.Net.Client;
using Jab;
using Spravy.Client.Helpers;
using Spravy.Client.Interfaces;
using Spravy.Client.Models;
using Spravy.Core.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Schedule.Domain.Client.Models;
using Spravy.Schedule.Domain.Client.Services;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Protos;

namespace Spravy.Schedule.Domain.Client.Modules;

[ServiceProviderModule]
[Singleton(typeof(GrpcScheduleServiceOptions), Factory = nameof(GrpcScheduleServiceOptionsFactory))]
[Singleton(
    typeof(IFactory<Uri, ScheduleService.ScheduleServiceClient>),
    Factory = nameof(ScheduleServiceClientsFactory)
)]
[Transient(typeof(IScheduleService), Factory = nameof(ScheduleServiceFactory))]
public interface IScheduleClientModule
{
    static IScheduleClientModule()
    {
        GrpcClientFactoryHelper.ClientFactories.Add(
            typeof(ScheduleService.ScheduleServiceClient),
            new ScheduleServiceClientFactory()
        );
    }

    static GrpcScheduleServiceOptions GrpcScheduleServiceOptionsFactory(
        ISerializer serializer,
        IConfigurationLoader configurationLoader
    )
    {
        using var stream = configurationLoader.GetStream();
        var configuration = serializer.Deserialize<GrpcScheduleServiceOptionsConfiguration>(stream);

        return configuration.ThrowIfError().GrpcScheduleService.ThrowIfNull();
    }

    static IFactory<Uri, ScheduleService.ScheduleServiceClient> ScheduleServiceClientsFactory(
        ClientOptions options,
        GrpcScheduleServiceOptions serviceOptions,
        ICacheValidator<Uri, GrpcChannel> cacheValidator
    )
    {
        if (options.UseCache)
        {
            return GrpcClientFactoryHelper.CreateCacheGrpcFactory<
                GrpcScheduleService,
                ScheduleService.ScheduleServiceClient,
                GrpcScheduleServiceOptions
            >(serviceOptions, cacheValidator);
        }

        return GrpcClientFactoryHelper.CreateGrpcFactory<
            GrpcScheduleService,
            ScheduleService.ScheduleServiceClient,
            GrpcScheduleServiceOptions
        >(serviceOptions);
    }

    static IScheduleService ScheduleServiceFactory(
        GrpcScheduleServiceOptions options,
        IFactory<Uri, ScheduleService.ScheduleServiceClient> grpcClientFactory,
        IRpcExceptionHandler handler,
        IMetadataFactory metadataFactory,
        IRetryService retryService
    )
    {
        return GrpcClientFactoryHelper.CreateGrpcServiceAuth<
            GrpcScheduleService,
            ScheduleService.ScheduleServiceClient,
            GrpcScheduleServiceOptions
        >(options, grpcClientFactory, handler, metadataFactory, retryService);
    }
}

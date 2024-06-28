using Grpc.Net.Client;
using Jab;
using Microsoft.Extensions.Configuration;
using Spravy.Client.Helpers;
using Spravy.Client.Models;
using Spravy.Core.Extensions;
using Spravy.Core.Interfaces;
using Spravy.PasswordGenerator.Domain.Client.Models;
using Spravy.PasswordGenerator.Domain.Client.Services;

namespace Spravy.PasswordGenerator.Domain.Client.Modules;

[ServiceProviderModule]
[Singleton(typeof(GrpcPasswordServiceOptions), Factory = nameof(GrpcPasswordServiceOptionsFactory))]
[Singleton(
    typeof(IFactory<Uri, PasswordServiceClient>),
    Factory = nameof(PasswordServiceClientsFactory)
)]
[Transient(typeof(IPasswordService), Factory = nameof(PasswordServiceFactory))]
public interface IPasswordGeneratorClientModule
{
    static GrpcPasswordServiceOptions GrpcPasswordServiceOptionsFactory(
        IConfiguration configuration
    )
    {
        return configuration.GetOptionsValue<GrpcPasswordServiceOptions>();
    }

    static IFactory<Uri, PasswordServiceClient> PasswordServiceClientsFactory(
        ClientOptions options,
        GrpcPasswordServiceOptions serviceOptions,
        ICacheValidator<Uri, GrpcChannel> cacheValidator
    )
    {
        if (options.UseCache)
        {
            return GrpcClientFactoryHelper.CreateCacheGrpcFactory<
                GrpcPasswordService,
                PasswordServiceClient,
                GrpcPasswordServiceOptions
            >(serviceOptions, cacheValidator);
        }

        return GrpcClientFactoryHelper.CreateGrpcFactory<
            GrpcPasswordService,
            PasswordServiceClient,
            GrpcPasswordServiceOptions
        >(serviceOptions);
    }

    static IPasswordService PasswordServiceFactory(
        GrpcPasswordServiceOptions options,
        IFactory<Uri, PasswordServiceClient> grpcClientFactory,
        IRpcExceptionHandler handler,
        IMetadataFactory metadataFactory
    )
    {
        return GrpcClientFactoryHelper.CreateGrpcServiceAuth<
            GrpcPasswordService,
            PasswordServiceClient,
            GrpcPasswordServiceOptions
        >(options, grpcClientFactory, handler, metadataFactory);
    }
}

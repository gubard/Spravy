using Grpc.Net.Client;
using Jab;
using Microsoft.Extensions.Configuration;
using Spravy.Authentication.Domain.Client.Models;
using Spravy.Authentication.Domain.Client.Services;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Client.Helpers;
using Spravy.Client.Models;
using Spravy.Core.Extensions;
using Spravy.Core.Interfaces;
using Spravy.Domain.Interfaces;
using static Spravy.Authentication.Protos.AuthenticationService;

namespace Spravy.Authentication.Domain.Client.Modules;

[ServiceProviderModule]
[Singleton(
    typeof(GrpcAuthenticationServiceOptions),
    Factory = nameof(GrpcAuthenticationServiceOptionsFactory)
)]
[Singleton(
    typeof(IFactory<Uri, AuthenticationServiceClient>),
    Factory = nameof(AuthenticationServiceClientsFactory)
)]
[Transient(typeof(IAuthenticationService), Factory = nameof(AuthenticationServiceFactory))]
public interface IAuthenticationClientModule
{
    static GrpcAuthenticationServiceOptions GrpcAuthenticationServiceOptionsFactory(
        IConfiguration configuration
    )
    {
        return configuration.GetOptionsValue<GrpcAuthenticationServiceOptions>();
    }

    static IFactory<Uri, AuthenticationServiceClient> AuthenticationServiceClientsFactory(
        ClientOptions options,
        GrpcAuthenticationServiceOptions serviceOptions,
        ICacheValidator<Uri, GrpcChannel> cacheValidator
    )
    {
        if (options.UseCache)
        {
            return GrpcClientFactoryHelper.CreateCacheGrpcFactory<
                GrpcAuthenticationService,
                AuthenticationServiceClient,
                GrpcAuthenticationServiceOptions
            >(serviceOptions, cacheValidator);
        }

        return GrpcClientFactoryHelper.CreateGrpcFactory<
            GrpcAuthenticationService,
            AuthenticationServiceClient,
            GrpcAuthenticationServiceOptions
        >(serviceOptions);
    }

    static IAuthenticationService AuthenticationServiceFactory(
        GrpcAuthenticationServiceOptions options,
        IFactory<Uri, AuthenticationServiceClient> grpcClientFactory,
        IRpcExceptionHandler handler
    )
    {
        return GrpcClientFactoryHelper.CreateGrpcService<
            GrpcAuthenticationService,
            AuthenticationServiceClient,
            GrpcAuthenticationServiceOptions
        >(options, grpcClientFactory, handler);
    }
}

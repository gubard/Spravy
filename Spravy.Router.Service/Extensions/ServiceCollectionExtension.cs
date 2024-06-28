extern alias AuthenticationToDo;
extern alias AuthenticationEventBus;
extern alias AuthenticationSchedule;
extern alias AuthenticationClient;
using System.Text.Json.Serialization;
using AuthenticationClient::Spravy.Authentication.Domain.Client.Models;
using AuthenticationClient::Spravy.Authentication.Domain.Client.Services;
using AuthenticationClient::Spravy.Authentication.Protos;
using AuthenticationEventBus::Spravy.EventBus.Domain.Client.Models;
using AuthenticationEventBus::Spravy.EventBus.Domain.Client.Services;
using AuthenticationSchedule::Spravy.Schedule.Domain.Client.Models;
using AuthenticationSchedule::Spravy.Schedule.Domain.Client.Services;
using AuthenticationSchedule::Spravy.Schedule.Protos;
using AuthenticationToDo::Spravy.ToDo.Domain.Client.Models;
using AuthenticationToDo::Spravy.ToDo.Domain.Client.Services;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Services;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Core.Interfaces;
using Spravy.Core.Services;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.Router.Service.Services;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Service.Services;
using Spravy.ToDo.Domain.Interfaces;
using Protos_EventBusService = AuthenticationEventBus::Spravy.EventBus.Protos.EventBusService;
using ToDoService = AuthenticationToDo::Spravy.ToDo.Protos.ToDoService;

namespace Spravy.Router.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterRouter(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddGrpcServiceAuth<
            GrpcToDoService,
            ToDoService.ToDoServiceClient,
            GrpcToDoServiceOptions
        >();
        serviceCollection.AddTransient<IEventBusService>(sp =>
            sp.GetRequiredService<GrpcEventBusService>()
        );
        serviceCollection.AddTransient<IScheduleService>(sp =>
            sp.GetRequiredService<GrpcScheduleService>()
        );
        serviceCollection.AddTransient<IToDoService>(sp =>
            sp.GetRequiredService<GrpcToDoService>()
        );
        serviceCollection.AddTransient<IRpcExceptionHandler, RpcExceptionHandler>();
        serviceCollection.AddTransient<ITokenService, TokenService>();
        serviceCollection.AddTransient<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<ContextAccessorUserIdHttpHeaderFactory>();
        serviceCollection.AddSingleton<ContextAccessorAuthorizationHttpHeaderFactory>();
        serviceCollection.AddSingleton<ContextTimeZoneOffsetHttpHeaderFactory>();
        serviceCollection.AddTransient<ISerializer, SpravyJsonSerializer>();
        serviceCollection.AddTransient<JsonSerializerContext, SpravyJsonSerializerContext>();

        serviceCollection.AddGrpcService<
            GrpcAuthenticationService,
            AuthenticationService.AuthenticationServiceClient,
            GrpcAuthenticationServiceOptions
        >();

        serviceCollection.AddGrpcServiceAuth<
            GrpcEventBusService,
            Protos_EventBusService.EventBusServiceClient,
            GrpcEventBusServiceOptions
        >();

        serviceCollection.AddGrpcServiceAuth<
            GrpcScheduleService,
            ScheduleService.ScheduleServiceClient,
            GrpcScheduleServiceOptions
        >();

        serviceCollection.AddTransient<IAuthenticationService>(sp =>
            sp.GetRequiredService<GrpcAuthenticationService>()
        );

        serviceCollection.AddTransient<IHttpHeaderFactory>(sp => new CombineHttpHeaderFactory(
            sp.GetRequiredService<ContextAccessorUserIdHttpHeaderFactory>(),
            sp.GetRequiredService<ContextAccessorAuthorizationHttpHeaderFactory>(),
            sp.GetRequiredService<ContextTimeZoneOffsetHttpHeaderFactory>()
        ));

        return serviceCollection;
    }
}

extern alias AuthenticationToDo;
extern alias AuthenticationEventBus;
extern alias AuthenticationSchedule;
extern alias AuthenticationClient;
using AuthenticationClient::Spravy.Authentication.Domain.Client.Models;
using AuthenticationClient::Spravy.Authentication.Domain.Client.Services;
using AuthenticationEventBus::Spravy.EventBus.Domain.Client.Models;
using AuthenticationEventBus::Spravy.EventBus.Domain.Client.Services;
using AuthenticationSchedule::Spravy.Schedule.Domain.Client.Models;
using AuthenticationSchedule::Spravy.Schedule.Domain.Client.Services;
using AuthenticationSchedule::Spravy.Schedule.Protos;
using AuthenticationToDo::Spravy.ToDo.Domain.Client.Models;
using AuthenticationToDo::Spravy.ToDo.Domain.Client.Services;
using AuthenticationToDo::Spravy.ToDo.Protos;
using Protos_AuthenticationService = AuthenticationClient::Spravy.Authentication.Protos.AuthenticationService;
using Protos_EventBusService = AuthenticationEventBus::Spravy.EventBus.Protos.EventBusService;

namespace Spravy.Router.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterRouter(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IRetryService, RetryService>();
        serviceCollection.AddGrpcServiceAuth<GrpcToDoService, ToDoService.ToDoServiceClient, GrpcToDoServiceOptions>();
        serviceCollection.AddTransient<IEventBusService>(sp => sp.GetRequiredService<GrpcEventBusService>());
        serviceCollection.AddTransient<IScheduleService>(sp => sp.GetRequiredService<GrpcScheduleService>());
        serviceCollection.AddTransient<IToDoService>(sp => sp.GetRequiredService<GrpcToDoService>());
        serviceCollection.AddTransient<IRpcExceptionHandler, RpcExceptionHandler>();
        serviceCollection.AddTransient<ITokenService, TokenService>();
        serviceCollection.AddTransient<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<ContextAccessorUserIdHttpHeaderFactory>();
        serviceCollection.AddSingleton<ContextAccessorAuthorizationHttpHeaderFactory>();
        serviceCollection.AddSingleton<ContextTimeZoneOffsetHttpHeaderFactory>();
        serviceCollection.AddTransient<ISerializer, SpravyJsonSerializer>();
        serviceCollection.AddTransient<JsonSerializerContext, SpravyJsonSerializerContext>();

        serviceCollection
           .AddTransient<IFactory<ChannelBase, Protos_AuthenticationService.AuthenticationServiceClient>,
                AuthenticationClientFactory>();

        serviceCollection
           .AddTransient<IFactory<ChannelBase, ScheduleService.ScheduleServiceClient>, ScheduleServiceClientFactory>();

        serviceCollection
           .AddTransient<IFactory<ChannelBase, ToDoService.ToDoServiceClient>, ToDoServiceClientFactory>();

        serviceCollection
           .AddGrpcService<GrpcAuthenticationService, Protos_AuthenticationService.AuthenticationServiceClient,
                GrpcAuthenticationServiceOptions>();

        serviceCollection
           .AddGrpcServiceAuth<GrpcEventBusService, Protos_EventBusService.EventBusServiceClient,
                GrpcEventBusServiceOptions>();

        serviceCollection
           .AddGrpcServiceAuth<GrpcScheduleService, ScheduleService.ScheduleServiceClient,
                GrpcScheduleServiceOptions>();

        serviceCollection.AddTransient<IAuthenticationService>(
            sp => sp.GetRequiredService<GrpcAuthenticationService>()
        );

        serviceCollection.AddTransient<IHttpHeaderFactory>(
            sp => new CombineHttpHeaderFactory(
                sp.GetRequiredService<ContextAccessorUserIdHttpHeaderFactory>(),
                sp.GetRequiredService<ContextAccessorAuthorizationHttpHeaderFactory>(),
                sp.GetRequiredService<ContextTimeZoneOffsetHttpHeaderFactory>()
            )
        );

        return serviceCollection;
    }
}
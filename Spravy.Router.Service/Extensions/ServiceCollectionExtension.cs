extern alias AuthenticationClient;
extern alias AuthenticationEventBus;
extern alias AuthenticationSchedule;
extern alias AuthenticationToDo;
using AuthenticationClient::Spravy.Authentication.Domain.Client.Models;
using AuthenticationClient::Spravy.Authentication.Domain.Client.Services;
using AuthenticationEventBus::Spravy.EventBus.Domain.Client.Models;
using AuthenticationEventBus::Spravy.EventBus.Domain.Client.Services;
using AuthenticationEventBus::Spravy.EventBus.Protos;
using AuthenticationSchedule::Spravy.Schedule.Domain.Client.Models;
using AuthenticationSchedule::Spravy.Schedule.Domain.Client.Services;
using AuthenticationSchedule::Spravy.Schedule.Protos;
using AuthenticationToDo::Spravy.ToDo.Domain.Client.Models;
using AuthenticationToDo::Spravy.ToDo.Domain.Client.Services;
using AuthenticationToDo::Spravy.ToDo.Protos;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Mapper.Profiles;
using Spravy.Authentication.Domain.Services;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Domain.Mapper.Profiles;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Mapper.Profiles;
using Spravy.Service.Extensions;
using Spravy.Service.Services;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Mapper.Profiles;
using AuthenticationService = AuthenticationClient::Spravy.Authentication.Protos.AuthenticationService;

namespace Spravy.Router.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterRouter(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddMapperConfiguration<SpravyAuthenticationProfile, SpravyEventBusProfile, SpravyScheduleProfile,
                SpravyToDoProfile>();

        serviceCollection
            .AddGrpcService2<GrpcAuthenticationService, AuthenticationService.AuthenticationServiceClient,
                GrpcAuthenticationServiceOptions>();
        serviceCollection
            .AddGrpcService<GrpcEventBusService, EventBusService.EventBusServiceClient, GrpcEventBusServiceOptions>();
        serviceCollection
            .AddGrpcService<GrpcScheduleService, ScheduleService.ScheduleServiceClient, GrpcScheduleServiceOptions>();
        serviceCollection
            .AddGrpcService<GrpcToDoService, ToDoService.ToDoServiceClient, GrpcToDoServiceOptions>();

        serviceCollection.AddTransient<IAuthenticationService>(
            sp => sp.GetRequiredService<GrpcAuthenticationService>()
        );

        serviceCollection.AddTransient<IEventBusService>(sp => sp.GetRequiredService<GrpcEventBusService>());
        serviceCollection.AddTransient<IScheduleService>(sp => sp.GetRequiredService<GrpcScheduleService>());
        serviceCollection.AddTransient<IToDoService>(sp => sp.GetRequiredService<GrpcToDoService>());
        serviceCollection.AddTransient<ITokenService, TokenService>();
        serviceCollection.AddTransient<IMetadataFactory, MetadataFactory>();
        serviceCollection.AddSingleton<TimeZoneHttpHeaderFactory>();
        serviceCollection.AddTransient<IHttpHeaderFactory>(
            sp => new CombineHttpHeaderFactory(
                sp.GetRequiredService<ContextAccessorUserIdHttpHeaderFactory>(),
                sp.GetRequiredService<TimeZoneHttpHeaderFactory>()
            )
        );

        return serviceCollection;
    }
}
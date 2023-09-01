using Spravy.EventBus.Domain.Mapper.Profiles;
using Spravy.EventBus.Service.Interfaces;
using Spravy.EventBus.Service.Services;
using Spravy.Service.Extensions;

namespace Spravy.EventBus.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddEventBus(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IEventPusher, EventPusher>();
        serviceCollection.AddMapperConfiguration<SpravyEventBusProfile>();

        return serviceCollection;
    }
}
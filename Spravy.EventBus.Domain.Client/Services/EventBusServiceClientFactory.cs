using Grpc.Core;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.EventBus.Protos;

namespace Spravy.EventBus.Domain.Client.Services;

public class EventBusServiceClientFactory
    : IFactory<ChannelBase, EventBusService.EventBusServiceClient>
{
    public Result<EventBusService.EventBusServiceClient> Create(ChannelBase key)
    {
        return new(new EventBusService.EventBusServiceClient(key));
    }
}

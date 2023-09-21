using Spravy.EventBus.Domain.Models;

namespace Spravy.EventBus.Domain.Interfaces;

public interface IEventBusService
{
    IAsyncEnumerable<EventValue> SubscribeEventsAsync(Guid[] eventIds, CancellationToken cancellationToken);
    Task PublishEventAsync(Guid eventId, byte[] content);
}
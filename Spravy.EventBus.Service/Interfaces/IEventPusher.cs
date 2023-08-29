using Spravy.EventBus.Domain.Models;

namespace Spravy.EventBus.Service.Interfaces;

public interface IEventPusher
{
    Task<EventValue> WaitEventAsync();
    Task PublishEventAsync(Guid eventId, Stream content);
}
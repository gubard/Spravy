using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Service.Interfaces;

namespace Spravy.EventBus.Service.Services;

public class EventPusher : IEventPusher
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private static EventValue value;

    public async Task<EventValue> WaitEventAsync()
    {
        await Semaphore.WaitAsync();

        return value;
    }

    public async Task PublishEventAsync(Guid eventId, Stream content)
    {
        var memoryStream = new MemoryStream();
        await content.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        value = new EventValue(eventId, memoryStream);
        Semaphore.Release();
    }
}
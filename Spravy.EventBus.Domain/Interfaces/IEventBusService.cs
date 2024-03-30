using Spravy.Domain.Models;
using Spravy.EventBus.Domain.Models;

namespace Spravy.EventBus.Domain.Interfaces;

public interface IEventBusService
{
    IAsyncEnumerable<EventValue> SubscribeEventsAsync(Guid[] eventIds, CancellationToken cancellationToken);
    Task<Result> PublishEventAsync(Guid eventId, byte[] content, CancellationToken cancellationToken);

    Task<Result<ReadOnlyMemory<EventValue>>> GetEventsAsync(
        ReadOnlyMemory<Guid> eventIds,
        CancellationToken cancellationToken
    );
}
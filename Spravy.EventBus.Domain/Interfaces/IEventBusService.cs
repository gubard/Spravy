using Spravy.Domain.Models;
using Spravy.EventBus.Domain.Models;

namespace Spravy.EventBus.Domain.Interfaces;

public interface IEventBusService
{
    IAsyncEnumerable<EventValue> SubscribeEventsAsync(Guid[] eventIds, CancellationToken cancellationToken);
    ValueTask<Result> PublishEventAsync(Guid eventId, byte[] content, CancellationToken cancellationToken);

    ValueTask<Result<ReadOnlyMemory<EventValue>>> GetEventsAsync(
        ReadOnlyMemory<Guid> eventIds,
        CancellationToken cancellationToken
    );
}
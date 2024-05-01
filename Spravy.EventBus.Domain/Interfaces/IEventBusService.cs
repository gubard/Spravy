using System.Runtime.CompilerServices;
using Spravy.Domain.Models;
using Spravy.EventBus.Domain.Models;

namespace Spravy.EventBus.Domain.Interfaces;

public interface IEventBusService
{
    ConfiguredCancelableAsyncEnumerable<Result<EventValue>> SubscribeEventsAsync(Guid[] eventIds, CancellationToken cancellationToken);

    ConfiguredValueTaskAwaitable<Result> PublishEventAsync(
        Guid eventId,
        byte[] content,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<EventValue>>> GetEventsAsync(
        ReadOnlyMemory<Guid> eventIds,
        CancellationToken cancellationToken
    );
}
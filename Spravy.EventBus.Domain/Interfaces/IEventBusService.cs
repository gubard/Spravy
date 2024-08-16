using System.Runtime.CompilerServices;
using Spravy.Domain.Models;
using Spravy.EventBus.Domain.Models;

namespace Spravy.EventBus.Domain.Interfaces;

public interface IEventBusService
{
    ConfiguredValueTaskAwaitable<Result> PublishEventAsync(
        Guid eventId,
        ReadOnlyMemory<byte> content,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<EventValue>>> GetEventsAsync(
        ReadOnlyMemory<Guid> eventIds,
        CancellationToken ct
    );
}

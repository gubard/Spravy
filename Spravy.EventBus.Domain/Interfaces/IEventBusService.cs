namespace Spravy.EventBus.Domain.Interfaces;

public interface IEventBusService
{
    Cvtar PublishEventAsync(Guid eventId, ReadOnlyMemory<byte> content, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<EventValue>>> GetEventsAsync(
        ReadOnlyMemory<Guid> eventIds,
        CancellationToken ct
    );
}
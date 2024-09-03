namespace Spravy.Ui.Features.Schedule.Interfaces;

public interface IEventViewModel : IStateHolder
{
    Guid Id { get; }

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<byte>>> GetContentAsync(
        CancellationToken ct
    );
}

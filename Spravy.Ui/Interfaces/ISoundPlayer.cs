namespace Spravy.Ui.Interfaces;

public interface ISoundPlayer
{
    Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct);
}
namespace Spravy.Sound;

public interface ISoundPlayer
{
    Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct);
}

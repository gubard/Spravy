namespace Spravy.Integration.Tests.Services;

public class EmptySoundPlayer : ISoundPlayer
{
    public Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
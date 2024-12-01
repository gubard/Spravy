namespace Spravy.Ui.Browser.Services;

public class SoundPlayer : ISoundPlayer
{
    public Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        JsAudioInterop.Play(soundData.ToArray());

        return Task.CompletedTask;
    }
}
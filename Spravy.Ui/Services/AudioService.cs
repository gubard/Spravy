using Avalonia.Platform;
using NAudio.Wave;

namespace Spravy.Ui.Services;

public class AudioService : IAudioService
{
    private const string AudioFileCompletePath = "avares://Spravy.Ui/Assets/Sounds/Complete.mp3";
    private static readonly Uri audioFileCompleteUri = new(AudioFileCompletePath);

    public Cvtar PlayCompleteAsync(CancellationToken ct)
    {
        return PlayCompleteCore(ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> PlayCompleteCore(CancellationToken ct)
    {
        await using var stream = AssetLoader.Open(audioFileCompleteUri);
        await using var reader = new Mp3FileReader(stream);
        reader.Skip(0.1);
        using var outputDevice = new WaveOutEvent();
        outputDevice.Init(reader);
        outputDevice.Play();
        await Task.Delay(TimeSpan.FromMilliseconds(500), ct);
        outputDevice.Stop();

        return Result.Success;
    }
}

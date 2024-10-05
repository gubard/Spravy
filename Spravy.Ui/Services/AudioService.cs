using Avalonia.Platform;
using Spravy.Sound;

namespace Spravy.Ui.Services;

public class AudioService : IAudioService
{
    private const string AudioFileCompletePath = "avares://Spravy.Ui/Assets/Sounds/Complete.wav";
    private static readonly Uri audioFileCompleteUri = new(AudioFileCompletePath);
    private static readonly ReadOnlyMemory<byte> completeSoundData;

    static AudioService()
    {
        using var stream = AssetLoader.Open(audioFileCompleteUri);
        stream.Position = (int)(44100 * 2 * 2 * .1);
        var buffer = new byte[(int)(44100 * 2 * 2 * .5)];
        var read = stream.Read(buffer);
        completeSoundData = buffer;
    }

    private readonly ISoundPlayer soundPlayer;

    public AudioService(ISoundPlayer soundPlayer)
    {
        this.soundPlayer = soundPlayer;
    }

    public Cvtar PlayCompleteAsync(CancellationToken ct)
    {
        return PlayCompleteCore(ct).ConfigureAwait(false);

        //return Result.AwaitableSuccess;
    }

    private async ValueTask<Result> PlayCompleteCore(CancellationToken ct)
    {
        await soundPlayer.PlayAsync(completeSoundData, ct);

        return Result.Success;
    }
}

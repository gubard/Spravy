using Avalonia.Platform;
using ISoundPlayer = Spravy.Ui.Interfaces.ISoundPlayer;

namespace Spravy.Ui.Services;

public class AudioService : IAudioService
{
    private const string AudioFileCompletePath = "avares://Spravy.Ui/Assets/Sounds/Complete.wav";
    private static readonly Uri audioFileCompleteUri = new(AudioFileCompletePath);
    private static readonly ReadOnlyMemory<byte> completeSoundData;

    static AudioService()
    {
        using var stream = AssetLoader.Open(audioFileCompleteUri);
        var span = stream.ToByteArray();
        completeSoundData = span.ToArray();
    }

    private readonly ISoundPlayer soundPlayer;

    public AudioService(ISoundPlayer soundPlayer)
    {
        this.soundPlayer = soundPlayer;
    }

    public Cvtar PlayCompleteAsync(CancellationToken ct)
    {
        return PlayCompleteCore(ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> PlayCompleteCore(CancellationToken ct)
    {
        await soundPlayer.PlayAsync(completeSoundData, ct);

        return Result.Success;
    }
}

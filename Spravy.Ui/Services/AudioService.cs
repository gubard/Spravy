namespace Spravy.Ui.Services;

public class AudioService : IAudioService
{
    private const string AudioFileCompletePath = "avares://Spravy.Ui/Assets/Sounds/Complete.wav";
    private const string AudioFileNotificationPath = "avares://Spravy.Ui/Assets/Sounds/Notification.wav";

    private static readonly Uri audioFileCompleteUri = new(AudioFileCompletePath);
    private static readonly ReadOnlyMemory<byte> completeSoundData;
    private static readonly Uri audioFileNotificationUri = new(AudioFileNotificationPath);
    private static readonly ReadOnlyMemory<byte> notificationSoundData;

    private readonly ISoundPlayer soundPlayer;
    private readonly SoundSettingsNotify soundSettingsNotify;

    static AudioService()
    {
        completeSoundData = audioFileCompleteUri.GetAssetBytes();
        notificationSoundData = audioFileNotificationUri.GetAssetBytes();
    }

    public AudioService(ISoundPlayer soundPlayer, SoundSettingsNotify soundSettingsNotify)
    {
        this.soundPlayer = soundPlayer;
        this.soundSettingsNotify = soundSettingsNotify;
    }

    public Cvtar PlayCompleteAsync(CancellationToken ct)
    {
        return PlayCore(completeSoundData, ct).ConfigureAwait(false);
    }

    public Cvtar PlayNotificationAsync(CancellationToken ct)
    {
        return PlayCore(notificationSoundData, ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> PlayCore(ReadOnlyMemory<byte> data, CancellationToken ct)
    {
        if (soundSettingsNotify.IsMute)
        {
            return Result.Success;
        }

        await soundPlayer.PlayAsync(data, ct);

        return Result.Success;
    }
}
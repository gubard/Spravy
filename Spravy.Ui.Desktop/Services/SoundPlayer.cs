using Spravy.Authentication.Domain.Interfaces;
using Spravy.Domain.Enums;
using Spravy.Domain.Helpers;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Services;

namespace Spravy.Ui.Desktop.Services;

public class SoundPlayer : ISoundPlayer
{
    private readonly ISoundPlayer soundPlayer;

    public SoundPlayer(IHashService hashService)
    {
        soundPlayer = OsHelper.Os switch
        {
            Os.Windows => new WindowsSoundPlayer(),
            Os.Linux => new LinuxSoundPlayer(),
            //Os.MacOs => new OsxSoundPlayer(hashService),
            _ => new EmptySoundPlayer(),
        };
    }

    public Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        return soundPlayer.PlayAsync(soundData, ct);
    }
}
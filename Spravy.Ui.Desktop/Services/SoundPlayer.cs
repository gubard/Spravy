using System.Runtime.InteropServices;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Desktop.Services;

public class SoundPlayer : ISoundPlayer
{
    private readonly ISoundPlayer soundPlayer;

    public SoundPlayer(IHashService hashService)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            soundPlayer = new WindowsSoundPlayer();

            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            soundPlayer = new LinuxSoundPlayer();

            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            soundPlayer = new OsxSoundPlayer(hashService);

            return;
        }

        throw new PlatformNotSupportedException(RuntimeInformation.OSDescription);
    }

    public Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        return soundPlayer.PlayAsync(soundData, ct);
    }
}
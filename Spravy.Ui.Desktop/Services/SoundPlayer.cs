﻿using System.Runtime.InteropServices;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Desktop.Services;

public class SoundPlayer : ISoundPlayer
{
    private readonly ISoundPlayer soundPlayer;

    public SoundPlayer()
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

        throw new PlatformNotSupportedException(RuntimeInformation.OSDescription);
    }

    public Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        return soundPlayer.PlayAsync(soundData, ct);
    }
}

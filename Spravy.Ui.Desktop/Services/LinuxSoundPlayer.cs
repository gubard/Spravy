using System.Runtime.InteropServices;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Desktop.Services;

public class LinuxSoundPlayer : ISoundPlayer
{
    private const int SND_PCM_STREAM_PLAYBACK = 0;
    private const int SND_PCM_FORMAT_S16_LE = 2;
    private const int SND_PCM_ACCESS_RW_INTERLEAVED = 3;

    public Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        return Task.Run(
            () =>
            {
                if (snd_pcm_open(out var pcm, "default", SND_PCM_STREAM_PLAYBACK, 0) < 0)
                {
                    throw new("Failed to open PCM device.");
                }

                try
                {
                    if (snd_pcm_set_params(
                            pcm,
                            SND_PCM_FORMAT_S16_LE,
                            SND_PCM_ACCESS_RW_INTERLEAVED,
                            2,
                            44100,
                            1,
                            500000
                        )
                      < 0)
                    {
                        throw new("Failed to set PCM parameters.");
                    }

                    snd_pcm_writei(pcm, soundData.ToArray(), soundData.Length / 4);
                    snd_pcm_drain(pcm);
                }
                finally
                {
                    snd_pcm_close(pcm);
                }
            },
            ct
        );
    }

    [DllImport("libasound.so")]
    private static extern int snd_pcm_open(out nint pcm, string name, int stream, int mode);

    [DllImport("libasound.so")]
    private static extern int snd_pcm_set_params(
        nint pcm,
        int format,
        int access,
        int channels,
        uint rate,
        int soft_resample,
        uint latency
    );

    [DllImport("libasound.so")]
    private static extern int snd_pcm_writei(nint pcm, byte[] buffer, int size);

    [DllImport("libasound.so")]
    private static extern int snd_pcm_drain(nint pcm);

    [DllImport("libasound.so")]
    private static extern int snd_pcm_close(nint pcm);
}
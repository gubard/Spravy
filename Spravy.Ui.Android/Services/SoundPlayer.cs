using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Media;

namespace Spravy.Ui.Android.Services;

public class SoundPlayer : ISoundPlayer, IDisposable
{
    private static readonly string completeAudioPath;
    public static readonly SoundPlayer Instance = new();

    static SoundPlayer()
    {
        completeAudioPath = Path.Combine(
            MainActivity.Instance.CacheDir.ThrowIfNull().AbsolutePath,
            "complete_audio.wav"
        );
    }

    private SoundPlayer() { }

    private readonly MediaPlayer mediaPlayer = new();

    public Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        if (mediaPlayer.IsPlaying)
        {
            mediaPlayer.Stop();
        }

        if (!File.Exists(completeAudioPath))
        {
            using var fileStream = new FileStream(
                completeAudioPath,
                FileMode.Create,
                FileAccess.Write
            );

            fileStream.Write(soundData.Span);
        }

        mediaPlayer.SetDataSource(completeAudioPath);
        mediaPlayer.Prepare();
        mediaPlayer.Start();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        File.Delete(completeAudioPath);
        mediaPlayer.Release();
        mediaPlayer.Dispose();
    }
}

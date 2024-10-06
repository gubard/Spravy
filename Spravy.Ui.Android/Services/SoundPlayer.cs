using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Media;

namespace Spravy.Ui.Android.Services;

public class SoundPlayer : ISoundPlayer, IDisposable
{
    private static readonly string completeAudioPath;
    private static readonly AndroidUri completeAudioUri;
    public static readonly SoundPlayer Instance = new();

    static SoundPlayer()
    {
        completeAudioPath = Path.Combine(
            MainActivity.Instance.CacheDir.ThrowIfNull().AbsolutePath,
            "complete_audio.wav"
        );

        completeAudioUri =
            AndroidUri.FromFile(new(completeAudioPath))
            ?? throw new ArgumentNullException(nameof(completeAudioUri));
    }

    private SoundPlayer() { }

    private MediaPlayer? mediaPlayer;

    public Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        DisposeMediaPlayer();

        if (!File.Exists(completeAudioPath))
        {
            using var fileStream = new FileStream(
                completeAudioPath,
                FileMode.Create,
                FileAccess.Write
            );

            fileStream.Write(soundData.Span);
        }

        mediaPlayer =
            MediaPlayer.Create(MainActivity.Instance, completeAudioUri)
            ?? throw new ArgumentNullException(nameof(mediaPlayer));

        mediaPlayer.SetDataSource(completeAudioPath);
        mediaPlayer.Prepare();
        mediaPlayer.Start();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        File.Delete(completeAudioPath);
        DisposeMediaPlayer();
    }

    private void DisposeMediaPlayer()
    {
        if (mediaPlayer is null)
        {
            return;
        }

        if (mediaPlayer.IsPlaying)
        {
            mediaPlayer.Stop();
        }

        mediaPlayer.Release();
        mediaPlayer.Dispose();
    }
}

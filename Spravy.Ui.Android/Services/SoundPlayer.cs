using Android.Media;

namespace Spravy.Ui.Android.Services;

public class SoundPlayer : ISoundPlayer, IDisposable
{
    private static readonly string audioDirectoryPath;
    public static readonly SoundPlayer Instance = new();

    static SoundPlayer()
    {
        audioDirectoryPath = Path.Combine(
            MainActivity.Instance.CacheDir.ThrowIfNull().AbsolutePath,
            "sounds"
        );
    }

    private SoundPlayer() { }

    private MediaPlayer? mediaPlayer;

    public async Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        DisposeMediaPlayer();

        var path = Path.Combine(
            audioDirectoryPath,
            $"{BitConverter.ToString(soundData.ToArray()[..255]).Replace("-", "")}.wav"
        );

        if (!File.Exists(path))
        {
            await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            fileStream.Write(soundData.Span);
        }

        mediaPlayer = new();
        await mediaPlayer.SetDataSourceAsync(path);
        mediaPlayer.Prepare();
        mediaPlayer.Start();
    }

    public void Dispose()
    {
        Directory.Delete(audioDirectoryPath);
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

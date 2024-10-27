using Android.Media;
using Spravy.Authentication.Domain.Interfaces;

namespace Spravy.Ui.Android.Services;

public class SoundPlayer : ISoundPlayer, IDisposable
{
    private readonly IHashService hashService;
    private static readonly string audioDirectoryPath;

    static SoundPlayer()
    {
        audioDirectoryPath = MainActivity.Instance.CacheDir.ThrowIfNull().AbsolutePath;
    }

    public SoundPlayer(IHashService hashService)
    {
        this.hashService = hashService;
    }

    private MediaPlayer? mediaPlayer;

    public async Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        DisposeMediaPlayer();
        var path = Path.Combine(
            audioDirectoryPath,
            $"{hashService.ComputeHash(soundData.ToArray()).ToHex()}.wav"
        );

        if (!File.Exists(path))
        {
            await using var fileStream = File.Create(path);
            await fileStream.WriteAsync(soundData, ct);
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

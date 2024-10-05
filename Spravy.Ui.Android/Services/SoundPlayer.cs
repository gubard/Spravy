using Android.Media;

namespace Spravy.Ui.Android.Services;

public class SoundPlayer : ISoundPlayer
{
    private static readonly string completeAudioPath = Path.Combine(
        MainActivity.Instance.CacheDir.ThrowIfNull().AbsolutePath,
        "temp_audio.mp3"
    );

    private readonly MediaPlayer mediaPlayer = new();

    public Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        mediaPlayer.Stop();

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
}

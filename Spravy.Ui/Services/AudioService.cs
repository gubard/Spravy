using Avalonia.Platform;
using NAudio.Wave;

namespace Spravy.Ui.Services;

public class AudioService : IAudioService
{
    public Result PlayComplete()
    {
        var uri = new Uri("Assets/Sounds/Complete.mp3", UriKind.RelativeOrAbsolute);
        using var stream = AssetLoader.Open(uri);
        using var audioFile = new Mp3FileReader(stream);
        using var outputDevice = new WaveOutEvent();
        outputDevice.Init(audioFile);
        outputDevice.Play();

        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            Thread.Sleep(100);
        }

        outputDevice.Stop();

        return Result.Success;
    }
}

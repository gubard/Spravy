using NAudio.Wave;

namespace Spravy.Ui.Extensions;

public static class WaveStreamExtensions
{
    public static void Skip(this WaveStream reader, double seconds)
    {
        var sampleRate = reader.WaveFormat.SampleRate;
        var channels = reader.WaveFormat.Channels;
        var bytesPerSample = reader.WaveFormat.BitsPerSample / 8;
        var bytesToSkip = (int)((sampleRate * channels * bytesPerSample) * seconds);
        reader.Position = bytesToSkip;
    }
}

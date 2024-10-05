using System.Runtime.InteropServices;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Desktop.Services;

public class WindowsSoundPlayer : ISoundPlayer
{
    [DllImport("winmm.dll", SetLastError = true)]
    public static extern int waveOutOpen(
        out nint hWaveOut,
        uint uDeviceID,
        ref WaveFormatEx lpFormat,
        WaveOutProc? dwCallback,
        nint dwInstance,
        uint dwFlags
    );

    [DllImport("winmm.dll", SetLastError = true)]
    public static extern int waveOutPrepareHeader(nint hWaveOut, nint lpWaveOutHdr, uint uSize);

    [DllImport("winmm.dll", SetLastError = true)]
    public static extern int waveOutWrite(nint hWaveOut, nint lpWaveOutHdr, uint uSize);

    [DllImport("winmm.dll", SetLastError = true)]
    public static extern int waveOutClose(nint hWaveOut);

    public delegate void WaveOutProc(
        nint hwo,
        uint uMsg,
        nint dwInstance,
        nint dwParam1,
        nint dwParam2
    );

    [StructLayout(LayoutKind.Sequential)]
    public struct WaveHeader
    {
        public nint lpData;
        public uint dwBufferLength;
        public uint dwBytesRecorded;
        public nint dwUser;
        public uint dwFlags;
        public uint dwLoops;
        public nint lpNext;
        public nint reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WaveFormatEx
    {
        public ushort wFormatTag;
        public ushort nChannels;
        public uint nSamplesPerSec;
        public uint nAvgBytesPerSec;
        public ushort nBlockAlign;
        public ushort wBitsPerSample;
        public ushort cbSize;
    }

    private const int MMSYSERR_NOERROR = 0;
    private const int CALLBACK_NULL = 0;
    private const int WAVE_FORMAT_PCM = 1;
    private const int WHDR_DONE = 0x00000001;

    private struct WaveHeaderOptions : IDisposable
    {
        private GCHandle handle;

        public WaveHeaderOptions(ReadOnlySpan<byte> soundData)
        {
            var waveFormat = new WaveFormatEx
            {
                wFormatTag = WAVE_FORMAT_PCM,
                nChannels = 2, // 2 for stereo, 1 for mono
                nSamplesPerSec = 44100, // 44.1 kHz sample rate
                wBitsPerSample = 16, // 16-bit samples
                nBlockAlign = 2 * 16 / 8,
                nAvgBytesPerSec = 44100 * 2 * 16 / 8,
                cbSize =
                    0 // No extra information
                ,
            };
            var result = waveOutOpen(
                out HWaveOut,
                0xFFFFFFFF,
                ref waveFormat,
                null,
                nint.Zero,
                CALLBACK_NULL
            );

            if (result != MMSYSERR_NOERROR)
            {
                throw new("Failed to open wave output device.");
            }

            // Prepare the wave header
            handle = GCHandle.Alloc(soundData.ToArray(), GCHandleType.Pinned);

            var header = new WaveHeader
            {
                lpData = handle.AddrOfPinnedObject(),
                dwBufferLength = (uint)soundData.Length,
                dwFlags = 0,
            };

            Header = new(header);

            result = waveOutPrepareHeader(
                HWaveOut,
                Header.Handle,
                (uint)Marshal.SizeOf(Header.Value)
            );

            if (result == MMSYSERR_NOERROR)
            {
                return;
            }

            Dispose();

            throw new("Failed to prepare wave header.");
        }

        public nint HWaveOut;
        public MarshalRef<WaveHeader> Header;

        public void Dispose()
        {
            handle.Free();
            Header.Dispose();
            var result = waveOutClose(HWaveOut);

            if (result != MMSYSERR_NOERROR)
            {
                throw new("Failed to close waveform.");
            }
        }
    }

    public async Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        using var options = new WaveHeaderOptions(soundData.Span);

        var result = waveOutWrite(
            options.HWaveOut,
            options.Header.Handle,
            (uint)Marshal.SizeOf(options.Header.Value)
        );

        if (result != MMSYSERR_NOERROR)
        {
            throw new("Failed to write waveform audio data.");
        }

        while ((options.Header.Value.dwFlags & WHDR_DONE) != WHDR_DONE)
        {
            await Task.Delay(100, ct);
        }
    }
}

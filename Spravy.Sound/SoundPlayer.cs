using System.Runtime.InteropServices;

namespace Spravy.Sound;

public class SoundPlayer : ISoundPlayer
{
    // Define the necessary Windows API methods and structures
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
        public nint lpData; // Pointer to waveform data
        public uint dwBufferLength; // Length of data buffer
        public uint dwBytesRecorded; // Used for input only
        public nint dwUser; // User data
        public uint dwFlags; // Flags
        public uint dwLoops; // Loop control counter
        public nint lpNext; // Reserved
        public nint reserved; // Reserved
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WaveFormatEx
    {
        public ushort wFormatTag; // Format category
        public ushort nChannels; // Number of channels
        public uint nSamplesPerSec; // Sampling rate
        public uint nAvgBytesPerSec; // For buffer estimation
        public ushort nBlockAlign; // Data block size
        public ushort wBitsPerSample; // Bits per sample
        public ushort cbSize; // Size of extra information (after this struct)
    }

    // Constants
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

            HeaderPtr = Marshal.AllocHGlobal(Marshal.SizeOf(header));
            Marshal.StructureToPtr(header, HeaderPtr, true);

            result = waveOutPrepareHeader(
                HWaveOut,
                HeaderPtr,
                (uint)Marshal.SizeOf(typeof(WaveHeader))
            );

            if (result == MMSYSERR_NOERROR)
            {
                return;
            }

            Dispose();

            throw new("Failed to prepare wave header.");
        }

        public nint HWaveOut;
        public nint HeaderPtr;

        public WaveHeader GetHeader()
        {
            return (WaveHeader)(
                Marshal.PtrToStructure(HeaderPtr, typeof(WaveHeader))
                ?? throw new NullReferenceException(nameof(HeaderPtr))
            );
        }

        public void Dispose()
        {
            handle.Free();
            Marshal.FreeHGlobal(HeaderPtr);
            var result = waveOutClose(HWaveOut);

            if (result != MMSYSERR_NOERROR)
            {
                throw new("Failed to close waveform.");
            }
        }
    }

    public async Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using var options = new WaveHeaderOptions(soundData.Span);

            var result = waveOutWrite(
                options.HWaveOut,
                options.HeaderPtr,
                (uint)Marshal.SizeOf(typeof(WaveHeader))
            );

            if (result != MMSYSERR_NOERROR)
            {
                throw new("Failed to write waveform audio data.");
            }

            while ((options.GetHeader().dwFlags & WHDR_DONE) != WHDR_DONE)
            {
                await Task.Delay(100, ct);
            }
        }
    }
}

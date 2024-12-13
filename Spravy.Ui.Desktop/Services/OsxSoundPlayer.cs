using System.Runtime.InteropServices;
using System.Text;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Desktop.Services;

public class OsxSoundPlayer : ISoundPlayer
{
    public OsxSoundPlayer(IHashService hashService)
    {
        this.hashService = hashService;
    }

    public async Task PlayAsync(ReadOnlyMemory<byte> soundData, CancellationToken ct)
    {
        var path = Path.Combine(audioDirectoryPath, $"{hashService.ComputeHash(soundData.ToArray()).ToHex()}.wav");

        if (!File.Exists(path))
        {
            await using var fileStream = File.Create(path);
            await fileStream.WriteAsync(soundData, ct);
        }

        var urlBytes = Encoding.UTF8.GetBytes(path);

        var cfUrl = CFURLCreateWithBytes(
            nint.Zero,
            urlBytes,
            urlBytes.Length,
            0x08000100,
            nint.Zero
        );

        if (cfUrl == nint.Zero)
        {
            return;
        }

        var player = AVAudioPlayer_initWithContentsOfURL(cfUrl, out var error);

        if (player == nint.Zero || error != nint.Zero)
        {
            return;
        }

        AVAudioPlayer_play(player);
        AVAudioPlayer_stop(player);
    }

    private static readonly string audioDirectoryPath = "storagery/audio/";

    private readonly IHashService hashService;

    // Import AVAudioPlayer from AVFoundation
    [DllImport("/System/Library/Frameworks/AVFoundation.framework/AVFoundation")]
    private static extern nint AVAudioPlayer_initWithContentsOfURL(nint url, out nint error);

    [DllImport("/System/Library/Frameworks/AVFoundation.framework/AVFoundation")]
    private static extern bool AVAudioPlayer_play(nint player);

    [DllImport("/System/Library/Frameworks/AVFoundation.framework/AVFoundation")]
    private static extern void AVAudioPlayer_stop(nint player);

    // Import CFURLCreateWithBytes to create a URL from a byte stream
    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static extern nint CFURLCreateWithBytes(
        nint allocator,
        byte[] bytes,
        long length,
        uint encoding,
        nint baseURL
    );
}
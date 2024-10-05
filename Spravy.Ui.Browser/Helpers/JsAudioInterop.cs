namespace Spravy.Ui.Browser.Helpers;

[SupportedOSPlatform("browser")]
public static partial class JsAudioInterop
{
    [JSImport("windowOpen", "audio.js")]
    public static partial void Play(byte[] audioData);
}

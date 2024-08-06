using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace Spravy.Ui.Browser.Helpers;

[SupportedOSPlatform("browser")]
public static partial class JsWindowInterop
{
    [JSImport("windowOpen", "window.js")]
    public static partial void WindowOpen(string url);

    [JSImport("getCurrentUrl", "window.js")]
    public static partial string GetCurrentUrl();
}

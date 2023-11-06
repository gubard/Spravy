using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace Spravy.Ui.Browser.Helpers;

[SupportedOSPlatform("browser")]
public static partial class JSInterop
{
    [JSImport("localStorageGetItem", "localStorage.js")]
    public static partial string LocalStorageGetItem(string key);

    [JSImport("localStorageSetItem", "localStorage.js")]
    public static partial void LocalStorageSetItem(string key, string value);

    [JSImport("localStorageRemoveItem", "localStorage.js")]
    public static partial void LocalStorageRemoveItem(string key);

    [JSImport("windowOpen", "window.js")]
    public static partial void WindowOpen(string url);
}
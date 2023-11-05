using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace Spravy.Ui.Browser.Helpers;

[SupportedOSPlatform("browser")]
public static partial class JSInterop
{
    [JSImport("localStorage.getItem")]
    public static partial string LocalStorageGetItem(string key);

    [JSImport("localStorage.setItem")]
    public static partial void LocalStorageSetItem(string key, string value);

    [JSImport("localStorage.removeItem")]
    public static partial void LocalStorageRemoveItem(string key);
}
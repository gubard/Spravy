namespace Spravy.Ui.Browser.Helpers;

[SupportedOSPlatform("browser")]
public static partial class JsLocalStorageInterop
{
    [JSImport("localStorageGetItem", "localStorage.js")]
    public static partial string LocalStorageGetItem(string key);

    [JSImport("localStorageSetItem", "localStorage.js")]
    public static partial void LocalStorageSetItem(string key, string value);

    [JSImport("localStorageRemoveItem", "localStorage.js")]
    public static partial void LocalStorageRemoveItem(string key);
}
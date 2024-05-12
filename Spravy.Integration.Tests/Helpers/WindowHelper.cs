namespace Spravy.Integration.Tests.Helpers;

public static class WindowHelper
{
    public static Window CreateWindow()
    {
        var window = DiHelper.Kernel.ThrowIfNull().Get<IDesktopTopLevelControl>() as Window;

        return window.ThrowIfNull();
    }
}
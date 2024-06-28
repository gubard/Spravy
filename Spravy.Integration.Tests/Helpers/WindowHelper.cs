using Spravy.Core.Helpers;

namespace Spravy.Integration.Tests.Helpers;

public static class WindowHelper
{
    public static Window CreateWindow()
    {
        var window = DiHelper.ServiceFactory.CreateService<IDesktopTopLevelControl>() as Window;

        return window.ThrowIfNull();
    }
}

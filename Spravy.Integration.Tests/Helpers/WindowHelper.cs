using Avalonia.Controls;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;

namespace Spravy.Integration.Tests.Helpers;

public static class WindowHelper
{
    public static Window CreateWindow()
    {
        var window = DiHelper.Kernel.ThrowIfNull().Get<IDesktopTopLevelControl>().As<Window>().ThrowIfNull();

        return window;
    }
}
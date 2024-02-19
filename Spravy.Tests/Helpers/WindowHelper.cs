using Avalonia.Controls;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;
using Xunit.Abstractions;

namespace Spravy.Tests.Helpers;

public static class WindowHelper
{
    public static Window CreateWindow(ITestOutputHelper output)
    {
        var window = DiHelper.Kernel.ThrowIfNull().Get<IDesktopTopLevelControl>().As<Window>().ThrowIfNull();
        output.WriteLine($"Create window: {window.Name}");

        return window;
    }
}
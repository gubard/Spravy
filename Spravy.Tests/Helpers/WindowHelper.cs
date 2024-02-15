using Avalonia.Controls;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;

namespace Spravy.Tests.Helpers;

public static class WindowHelper
{
    public static Window CreateWindow()
    {
        return DiHelper.Kernel.ThrowIfNull().Get<IDesktopTopLevelControl>().As<Window>().ThrowIfNull();
    }
}
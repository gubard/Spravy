using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;

namespace Spravy.Tests.Helpers;

public static class WindowHelper
{
    public static IReadOnlyList<Window> GetWindowList()
    {
        return Application.Current.ThrowIfNull()
            .ApplicationLifetime.ThrowIfNull()
            .ThrowIfIsNotCast<IClassicDesktopStyleApplicationLifetime>()
            .Windows;
    }

    public static Window CreateWindow()
    {
        return DiHelper.Kernel.ThrowIfNull().Get<Window>();
    }
}
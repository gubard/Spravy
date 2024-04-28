using Avalonia.Controls;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Views;

namespace Spravy.Integration.Tests.Helpers;

public static class WindowHelper
{
    public static Window CreateWindow()
    {
        var mainWindow = new MainWindow
        {
            Content = DiHelper.Kernel.ThrowIfNull().Get<Control>()
        };

        return mainWindow;
    }
}

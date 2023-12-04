using System;
using Avalonia.Threading;
using Spravy.Ui.Models;

namespace Spravy.Ui.Extensions;

public static class ViewModelExtension
{
    public static DispatcherOperation InvokeUIBackgroundAsync(this ViewModelBase _, Action callback)
    {
        return Dispatcher.UIThread.InvokeAsync(callback, DispatcherPriority.Background);
    }
}
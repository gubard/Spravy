using System;
using Avalonia.Threading;

namespace Spravy.Ui.Extensions;

public static class ObjectExtension
{
    public static DispatcherOperation InvokeUIBackgroundAsync<T>(this T _, Action callback)
    {
        return Dispatcher.UIThread.InvokeAsync(callback, DispatcherPriority.Background);
    }
    
    public static DispatcherOperation InvokeUIAsync<T>(this T _, Action callback)
    {
        return Dispatcher.UIThread.InvokeAsync(callback);
    }
}
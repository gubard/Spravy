using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Spravy.Ui.Extensions;

public static class ObjectExtension
{
    public static DispatcherOperation InvokeUIBackgroundAsync<TObject>(this TObject _, Action callback)
    {
        return Dispatcher.UIThread.InvokeAsync(callback, DispatcherPriority.Background);
    }

    public static DispatcherOperation<TResult> InvokeUIBackgroundAsync<TObject, TResult>(
        this TObject _,
        Func<TResult> callback
    )
    {
        return Dispatcher.UIThread.InvokeAsync(callback, DispatcherPriority.Background);
    }

    public static DispatcherOperation InvokeUIAsync<TObject>(this TObject _, Action callback)
    {
        return Dispatcher.UIThread.InvokeAsync(callback);
    }

    public static Task InvokeUIAsync<TObject>(this TObject _, Func<Task> callback)
    {
        return Dispatcher.UIThread.InvokeAsync(callback);
    }
}
namespace Spravy.Ui.Extensions;

public static class DispatcherExtension
{
    public static DispatcherOperation InvokeBackgroundAsync(
        this Dispatcher dispatcher,
        Action callback
    )
    {
        return Dispatcher.UIThread.InvokeAsync(callback);
    }
}

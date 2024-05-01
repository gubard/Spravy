namespace Spravy.Ui.Extensions;

public static class ObjectExtension
{
    public static ConfiguredValueTaskAwaitable<Result> InvokeUIBackgroundAsync<TObject>(this TObject _, Action callback)
    {
        return InvokeUIBackgroundCore(_, callback).ConfigureAwait(false);
    }

    private static async ValueTask<Result> InvokeUIBackgroundCore<TObject>(this TObject _, Action callback)
    {
        await Dispatcher.UIThread.InvokeAsync(callback, DispatcherPriority.Background);

        return Result.Success;
    }

    public static async ValueTask<Result> InvokeUIBackgroundAsync<TObject>(this TObject _, Func<Task> callback)
    {
        await Dispatcher.UIThread.InvokeAsync(callback, DispatcherPriority.Background);

        return Result.Success;
    }

    public static async ValueTask<Result<TResult>> InvokeUIBackgroundAsync<TObject, TResult>(
        this TObject _,
        Func<Result<TResult>> callback
    )
    {
        var result = await Dispatcher.UIThread.InvokeAsync(callback, DispatcherPriority.Background);

        return result;
    }
}
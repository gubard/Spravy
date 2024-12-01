namespace Spravy.Ui.Extensions;

public static class ObjectExtension
{
    public static Result PostUi<TObject>(this TObject _, Func<Result> func)
    {
        Dispatcher.UIThread.Post(() => func.Invoke(), DispatcherPriority.Default);

        return Result.Success;
    }

    public static Result PostUiBackground<TObject>(this TObject _, Func<Result> func, CancellationToken ct)
    {
        Dispatcher.UIThread.Post(
            () =>
            {
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                func.Invoke();
            },
            DispatcherPriority.Background
        );

        return Result.Success;
    }

    public static TValue GetUiValue<TObject, TValue>(this TObject _, Func<TValue> callback)
    {
        return Dispatcher.UIThread.Invoke(callback);
    }

    public static Cvtar InvokeUiAsync<TObject>(this TObject _, Func<Result> callback)
    {
        return _.InvokeUiCore(callback).ConfigureAwait(false);
    }

    private static async ValueTask<Result> InvokeUiCore<TObject>(this TObject _, Func<Result> callback)
    {
        var result = await Dispatcher.UIThread.InvokeAsync(callback);

        return result;
    }

    public static Cvtar InvokeUiBackgroundAsync<TObject>(this TObject _, Func<Result> callback)
    {
        return _.InvokeUiBackgroundCore(callback).ConfigureAwait(false);
    }

    private static async ValueTask<Result> InvokeUiBackgroundCore<TObject>(this TObject _, Func<Result> callback)
    {
        var result = await Dispatcher.UIThread.InvokeAsync(callback, DispatcherPriority.Background);

        return result;
    }

    public static ConfiguredValueTaskAwaitable<Result<TResult>> InvokeUiBackgroundAsync<TObject, TResult>(
        this TObject _,
        Func<Result<TResult>> callback
    ) where TResult : notnull
    {
        return _.InvokeUiBackgroundCore(callback).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TResult>> InvokeUiBackgroundCore<TObject, TResult>(
        this TObject _,
        Func<Result<TResult>> callback
    ) where TResult : notnull
    {
        var result = await Dispatcher.UIThread.InvokeAsync(callback, DispatcherPriority.Background);

        return result;
    }

    public static ConfiguredValueTaskAwaitable<Result<TResult>> InvokeUiAsync<TObject, TResult>(
        this TObject _,
        Func<Result<TResult>> callback
    ) where TResult : notnull
    {
        return _.InvokeUiCore(callback).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TResult>> InvokeUiCore<TObject, TResult>(
        this TObject _,
        Func<Result<TResult>> callback
    ) where TResult : notnull
    {
        var result = await Dispatcher.UIThread.InvokeAsync(callback);

        return result;
    }
}
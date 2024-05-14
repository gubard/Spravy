namespace Spravy.Ui.Extensions;

public static class ObjectExtension
{
    public static ConfiguredValueTaskAwaitable<Result> InvokeUiBackgroundAsync<TObject>(this TObject _, Action callback)
    {
        return InvokeUiBackgroundCore(_, callback).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> InvokeUiBackgroundCore<TObject>(this TObject _, Action callback)
    {
        await Dispatcher.UIThread.InvokeAsync(callback, DispatcherPriority.Background);
        
        return Result.Success;
    }
    
    public static async ValueTask<Result> InvokeUiBackgroundAsync<TObject>(this TObject _, Func<Task> callback)
    {
        await Dispatcher.UIThread.InvokeAsync(callback, DispatcherPriority.Background);
        
        return Result.Success;
    }
    
    public static ConfiguredValueTaskAwaitable<Result<TResult>> InvokeUiBackgroundAsync<TObject, TResult>(
        this TObject _,
        Func<Result<TResult>> callback
    )
    {
        return _.InvokeUiBackgroundCore(callback).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result<TResult>> InvokeUiBackgroundCore<TObject, TResult>(
        this TObject _,
        Func<Result<TResult>> callback
    )
    {
        var result = await Dispatcher.UIThread.InvokeAsync(callback, DispatcherPriority.Background);
        
        return result;
    }
}
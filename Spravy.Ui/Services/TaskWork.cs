namespace Spravy.Ui.Services;

public class TaskWork
{
    private readonly Delegate del;
    private readonly IErrorHandler errorHandler;
    private CancellationTokenSource cancellationTokenSource = new();
    private ConfiguredValueTaskAwaitable<Result>? current;

    private TaskWork(Delegate del, IErrorHandler errorHandler)
    {
        this.del = del;
        this.errorHandler = errorHandler;
    }

    public ConfiguredValueTaskAwaitable<Result> Current
    {
        get => current.ThrowIfNullStruct();
    }

    public async Task RunAsync()
    {
        Cancel();
        var value = del.DynamicInvoke(cancellationTokenSource.Token).ThrowIfNull();
        current = (ConfiguredValueTaskAwaitable<Result>)value;
        var result = await Current;
        await errorHandler.ErrorsHandleAsync(result.Errors, CancellationToken.None);
    }

    public async Task RunAsync<T>(T value)
    {
        Cancel();
        var v = del.DynamicInvoke(value, cancellationTokenSource.Token).ThrowIfNull();
        current = (ConfiguredValueTaskAwaitable<Result>)v;
        var result = await Current;
        await errorHandler.ErrorsHandleAsync(result.Errors, CancellationToken.None);
    }

    public void Cancel()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
    }

    public static TaskWork Create(
        IErrorHandler errorHandler,
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> task
    )
    {
        return new(task, errorHandler);
    }

    public static TaskWork Create<T>(
        IErrorHandler errorHandler,
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable<Result>> task
    )
    {
        return new(task, errorHandler);
    }
}

using Spravy.Domain.Extensions;

namespace Spravy.Domain.Models;

public class TaskWork
{
    private readonly Delegate del;
    private CancellationTokenSource cancellationTokenSource = new();
    private Task? current;

    private TaskWork(Delegate del)
    {
        this.del = del;
    }

    public Task Current => current.ThrowIfNull();

    public Task RunAsync()
    {
        Cancel();
        current = (Task)del.DynamicInvoke(cancellationTokenSource.Token).ThrowIfNull();

        return current;
    }

    public Task RunAsync<T>(T value)
    {
        Cancel();
        current = (Task)del.DynamicInvoke(value, cancellationTokenSource.Token).ThrowIfNull();

        return current;
    }

    public void Cancel()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
    }

    public static TaskWork Create(Func<CancellationToken, Task> task)
    {
        return new(task);
    }

    public static TaskWork Create<T>(Func<T, CancellationToken, Task> task)
    {
        return new(task);
    }
}
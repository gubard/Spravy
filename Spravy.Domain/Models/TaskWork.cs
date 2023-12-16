using Spravy.Domain.Services;

namespace Spravy.Domain.Models;

public class TaskWork
{
    private readonly Func<CancellationToken, Task> task;
    private CancellationTokenSource cancellationTokenSource = new();
    private Task current; //= Task.CompletedTask;

    public TaskWork(Func<CancellationToken, Task> task)
    {
        this.task = task;
    }

    public Task Current => current;

    public Task RunAsync()
    {
        Cancel();
        current = task.Invoke(cancellationTokenSource.Token);

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

    public static TaskWork<T> Create<T>(Func<T, CancellationToken, Task> task)
    {
        return new(task);
    }
}

public class TaskWork<T>
{
    private readonly Func<T, CancellationToken, Task> task;
    private readonly CancellationTokenFactory cancellationTokenFactory = new();

    public TaskWork(Func<T, CancellationToken, Task> task)
    {
        this.task = task;
    }

    public Task RunAsync(T value)
    {
        Cancel();

        return task.Invoke(value, cancellationTokenFactory.Token);
    }

    public void Cancel()
    {
        cancellationTokenFactory.Cancel();
        cancellationTokenFactory.Reset();
    }
}
using Spravy.Domain.Extensions;

namespace Spravy.Domain.Models;

public class TaskWork
{
    private static readonly List<Task> Tasks = new();
    private readonly Delegate del;
    private CancellationTokenSource cancellationTokenSource = new();
    private Task? current;

    private TaskWork(Delegate del)
    {
        this.del = del;
    }

    private static void UpdateTasks(Task task)
    {
        Tasks.Add(task);
        Tasks.RemoveAll(x => x.IsCompletedSuccessfully || x.IsCanceled || x.IsCompleted || x.IsFaulted);
    }

    public static Task AllTasks => Task.WhenAll(Tasks);

    public Task Current => current.ThrowIfNull();

    public Task RunAsync()
    {
        Cancel();
        current = (Task)del.DynamicInvoke(cancellationTokenSource.Token).ThrowIfNull();
        UpdateTasks(current);
        
        return current;
    }

    public Task RunAsync<T>(T value)
    {
        Cancel();
        current = (Task)del.DynamicInvoke(value, cancellationTokenSource.Token).ThrowIfNull();
        UpdateTasks(current);

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
using System.Runtime.CompilerServices;
using Spravy.Domain.Services;

namespace Spravy.Domain.Models;

public class TaskWork
{
    private readonly Func<CancellationToken, Task> task;
    private CancellationTokenSource cancellationTokenSource = new();
    private readonly List<TaskWork> subTasks = new();
    private Task current; //= Task.CompletedTask;

    public TaskWork(Func<CancellationToken, Task> task)
    {
        this.task = task;
    }

    public Task Current => current;

    public Task RunAsync()
    {
        Cancel();

        foreach (var subTask in subTasks)
        {
            subTask.Cancel();
        }

        current = task.Invoke(cancellationTokenSource.Token);

        return current;
    }

    public void Cancel()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
    }

    public void AddSubTasks(TaskWork taskWork)
    {
        subTasks.Add(taskWork);
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

public readonly struct TaskWork<T>
{
    private readonly Func<T, CancellationToken, Task> task;
    private readonly CancellationTokenFactory cancellationTokenFactory = new();
    private readonly List<TaskWork> subTasks = new();

    public TaskWork(Func<T, CancellationToken, Task> task)
    {
        this.task = task;
    }

    public ConfiguredTaskAwaitable RunAsync(T value)
    {
        Cancel();

        foreach (var subTask in subTasks)
        {
            subTask.Cancel();
        }

        return task.Invoke(value, cancellationTokenFactory.Token).ConfigureAwait(false);
    }

    public void Cancel()
    {
        cancellationTokenFactory.Cancel();
        cancellationTokenFactory.Reset();
    }

    public void AddSubTasks(TaskWork taskWork)
    {
        subTasks.Add(taskWork);
    }
}
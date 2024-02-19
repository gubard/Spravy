using Avalonia.Threading;
using Spravy.Domain.Models;
using Xunit.Abstractions;

namespace Spravy.Tests.Extensions;

public static class ObjectExtension
{
    private static readonly DispatcherPriority[] dispatcherPriorities;

    static ObjectExtension()
    {
        DispatcherPriority[] dp =
        {
            DispatcherPriority.SystemIdle,
            DispatcherPriority.ApplicationIdle,
            DispatcherPriority.ContextIdle,
            DispatcherPriority.Background,
            DispatcherPriority.Input,
            DispatcherPriority.Default,
            DispatcherPriority.Loaded,
            DispatcherPriority.Render,
            DispatcherPriority.Send,
        };

        dispatcherPriorities = dp.OrderBy(x => x.Value).ToArray();
    }

    public static async Task<TObject> WhenAllTasksAsync<TObject>(this TObject obj)
    {
        await TaskWork.AllTasks;

        return obj;
    }

    public static TObject WaitSeconds<TObject>(this TObject obj, byte seconds, ITestOutputHelper output)
    {
        for (var i = 0; i < seconds; i++)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            obj.RunJobsAll(output);
        }

        return obj;
    }

    public static TObject WaitSeconds<TObject>(
        this TObject obj,
        byte seconds,
        Action predicate,
        ITestOutputHelper output
    )
    {
        for (var i = 0; i < seconds; i++)
        {
            try
            {
                predicate.Invoke();

                return obj;
            }
            catch
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                obj.RunJobsAll(output);
            }
        }

        predicate.Invoke();

        return obj;
    }

    public static T WaitSeconds<TObject, T>(this TObject obj, byte seconds, Func<T> predicate, ITestOutputHelper output)
    {
        for (var i = 0; i < seconds; i++)
        {
            try
            {
                return predicate.Invoke();
            }
            catch
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                obj.RunJobsAll(output);
            }
        }

        return predicate.Invoke();
    }

    public static async Task<T> WaitSecondsAsync<TObject, T>(
        this TObject obj,
        byte seconds,
        Func<T> predicate,
        ITestOutputHelper output
    )
    {
        for (var i = 0; i < seconds; i++)
        {
            try
            {
                return predicate.Invoke();
            }
            catch
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                obj.RunJobsAll(output);
            }
        }

        return predicate.Invoke();
    }

    public static async Task<TObject> CaseAsync<TObject>(this TObject obj, Func<Task> action)
    {
        await action.Invoke();

        return obj;
    }

    public static async Task<TObject> DelayAsync<TObject>(this TObject obj, TimeSpan timeout)
    {
        await Task.Delay(timeout);

        return obj;
    }

    public static TObject RunJobsMinimumActiveValue<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs();

        return obj;
    }

    public static TObject RunJobsDefault<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Default);

        return obj;
    }

    public static TObject RunJobsInactive<TObject>(this TObject obj, ITestOutputHelper output)
    {
        output.WriteLine($"Running job {nameof(DispatcherPriority.Inactive)}");
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Inactive);
        output.WriteLine($"Running job {nameof(DispatcherPriority.Inactive)}");

        return obj;
    }

    public static TObject RunJobsInput<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Input);

        return obj;
    }

    public static TObject RunJobsBackground<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Background);

        return obj;
    }

    public static TObject RunJobsLoaded<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Loaded);

        return obj;
    }

    public static TObject RunJobsInvalid<TObject>(this TObject obj, ITestOutputHelper output)
    {
        output.WriteLine($"Running job {nameof(DispatcherPriority.Invalid)}");
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Invalid);
        output.WriteLine($"End job {nameof(DispatcherPriority.Invalid)}");

        return obj;
    }

    public static TObject RunJobsNormal<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Normal);

        return obj;
    }

    public static TObject RunJobsRender<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Render);

        return obj;
    }

    public static TObject RunJobsSend<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Send);

        return obj;
    }

    public static TObject RunJobsApplicationIdle<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.ApplicationIdle);

        return obj;
    }

    public static TObject RunJobsContextIdle<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.ContextIdle);

        return obj;
    }

    public static TObject RunJobsMaxValue<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.MaxValue);

        return obj;
    }

    public static TObject RunJobsSystemIdle<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.SystemIdle);

        return obj;
    }

    public static TObject RunJobsAll<TObject>(this TObject obj, ITestOutputHelper output)
    {
        var count = 0;
        obj.RunJobsInvalid(output);
        obj.RunJobsInactive(output);

        while (count != dispatcherPriorities.Length)
        {
            count = 0;

            foreach (var dispatcherPriority in dispatcherPriorities)
            {
                if (!Dispatcher.UIThread.HasJobsWithPriority(dispatcherPriority))
                {
                    count++;
                }
                else
                {
                    output.WriteLine($"Running job {dispatcherPriority.Value}");
                    Dispatcher.UIThread.RunJobs(dispatcherPriority);
                    output.WriteLine($"End job {dispatcherPriority.Value}");
                }
            }
        }

        return obj;
    }

    public static TObject TryCatch<TObject>(this TObject obj, Action<TObject> @try, Action<TObject, Exception> @catch)
    {
        try
        {
            @try.Invoke(obj);
        }
        catch (Exception e)
        {
            @catch.Invoke(obj, e);
            throw;
        }

        return obj;
    }

    public static async Task<TObject> TryCatchAsync<TObject>(
        this TObject obj,
        Func<TObject, Task> @try,
        Action<TObject, Exception> @catch
    )
    {
        try
        {
            await @try.Invoke(obj);
        }
        catch (Exception e)
        {
            @catch.Invoke(obj, e);
            throw;
        }

        return obj;
    }
}
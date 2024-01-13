using Avalonia.Threading;

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

    public static TObject WaitSeconds<TObject>(this TObject obj, byte seconds)
    {
        for (var i = 0; i < seconds; i++)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            obj.RunJobsAll();
        }

        return obj;
    }

    public static TObject Case<TObject>(this TObject obj, Action<TObject> action)
    {
        action.Invoke(obj);

        return obj;
    }

    public static TObject Case<TObject>(this TObject obj, Action action)
    {
        action.Invoke();

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

    public static TObject RunJobsInactive<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Inactive);

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

    public static TObject RunJobsInvalid<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Invalid);

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

    public static TObject RunJobsAll<TObject>(this TObject obj)
    {
        var count = 0;
        obj.RunJobsInvalid();
        obj.RunJobsInactive();

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
                    Dispatcher.UIThread.RunJobs(dispatcherPriority);
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
}
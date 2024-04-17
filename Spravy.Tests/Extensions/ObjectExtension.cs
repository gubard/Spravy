using Avalonia.Threading;

namespace Spravy.Tests.Extensions;

public static class ObjectExtension
{
    public static TObject RunJobsAll<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs();

        return obj;
    }

    public static TObject RunJobsAll<TObject>(this TObject obj, ulong count)
    {
        Console.WriteLine($"RunJobsAll start {count}");

        for (ulong i = 0; i < count; i++)
        {
            Dispatcher.UIThread.RunJobs();
        }

        Console.WriteLine($"RunJobsAll end {count}");

        return obj;
    }

    public static TObject RunJobsAll<TObject>(this TObject obj, TimeSpan waitTime)
    {
        Console.WriteLine("RunJobsAll Start");
        Dispatcher.UIThread.RunJobs();
        Dispatcher.UIThread.InvokeAsync(() => Task.Delay(waitTime), DispatcherPriority.MaxValue);
        Dispatcher.UIThread.RunJobs();
        Console.WriteLine("RunJobsAll End");

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
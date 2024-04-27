using Avalonia.Threading;

namespace Spravy.Tests.Extensions;

public static class ObjectExtension
{
    public static TObject RunJobsAll<TObject>(this TObject obj)
    {
        Dispatcher.UIThread.RunJobs();

        return obj;
    }

    public static TObject RunJobsAll<TObject>(this TObject obj, ulong seconds)
    {
        for (ulong i = 0; i < seconds; i++)
        {
            Dispatcher.UIThread.RunJobs();
            Dispatcher.UIThread.Post(() => Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false).GetAwaiter().GetResult());
            Dispatcher.UIThread.RunJobs();
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
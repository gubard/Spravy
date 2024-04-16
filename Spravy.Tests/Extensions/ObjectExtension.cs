using Avalonia.Threading;

namespace Spravy.Tests.Extensions;

public static class ObjectExtension
{
    public static TObject RunJobsAll<TObject>(this TObject obj, ulong count = 1)
    {
        for (ulong i = 0; i < count; i++)
        {
            Task.Yield().GetAwaiter().GetResult();
            Task.Delay(10).GetAwaiter().GetResult();
            Thread.Sleep(10);
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
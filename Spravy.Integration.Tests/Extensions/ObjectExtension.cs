namespace Spravy.Integration.Tests.Extensions;

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
            for (byte j = 0; j < 10; j++)
            {
                Dispatcher.UIThread.RunJobs();

                Dispatcher.UIThread.Post(
                    () =>
                        Task.Delay(TimeSpan.FromMilliseconds(100))
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult()
                );

                Dispatcher.UIThread.RunJobs();
            }
        }

        return obj;
    }

    public static TObject TryCatch<TObject>(
        this TObject obj,
        Action<TObject> @try,
        Action<TObject, Exception> @catch
    )
    {
        var a = obj;

        try
        {
            @try.Invoke(a);
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

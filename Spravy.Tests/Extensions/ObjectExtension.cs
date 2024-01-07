using Avalonia.Threading;

namespace Spravy.Tests.Extensions;

public static class ObjectExtension
{
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
        obj.RunJobsInvalid();
        obj.RunJobsInactive();
        obj.RunJobsSystemIdle();
        obj.RunJobsApplicationIdle();
        obj.RunJobsContextIdle();
        obj.RunJobsBackground();
        obj.RunJobsInput();
        obj.RunJobsDefault();
        obj.RunJobsLoaded();
        obj.RunJobsRender();
        obj.RunJobsSend();

        return obj;
    }
}
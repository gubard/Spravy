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
}
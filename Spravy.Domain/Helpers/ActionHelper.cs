namespace Spravy.Domain.Helpers;

public static class ActionHelper
{
    public static readonly Action Empty = () =>
    {
    };
}

public static class ActionHelper<T>
{
    public static readonly Action<T> Empty = _ =>
    {
    };

    public static readonly EventHandler<T> EmptyEventHandler = new(ActionHelper<object?, T>.Empty);
}

public static class ActionHelper<T, T2>
{
    public static readonly Action<T, T2> Empty = (_, _) =>
    {
    };
}
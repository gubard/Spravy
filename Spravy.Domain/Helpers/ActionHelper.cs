namespace Spravy.Domain.Helpers;

public static class ActionHelper
{
    public static readonly Action Empty = () => { };
}

public static class ActionHelper<T>
{
    public static readonly Action<T> Empty = _ => { };
}
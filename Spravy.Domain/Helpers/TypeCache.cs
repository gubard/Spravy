namespace Spravy.Domain.Helpers;

public static class TypeCache<T>
{
    static TypeCache()
    {
        Type = typeof(T);
    }

    public static Type Type { get; }
}

namespace Spravy.Domain.Helpers;

public static class TypeCache<T>
{
    static TypeCache()
    {
        Type = typeof(T);
        Name = Type.Name;
    }

    public static readonly Type Type;
    public static string Name;
}
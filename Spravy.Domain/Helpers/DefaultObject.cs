namespace Spravy.Domain.Helpers;

public static class DefaultObject<T> where T : class, new()
{
    public static readonly T Default = new();
}
namespace Spravy.Domain.Extensions;

public static class ReadOnlyCollectionExtension
{
    public static bool IsSingle<T>(this ReadOnlyCollection<T> collection)
    {
        if (collection.Count == 1)
        {
            return true;
        }

        return false;
    }
}
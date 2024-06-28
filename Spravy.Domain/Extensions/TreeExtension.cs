namespace Spravy.Domain.Extensions;

public static class TreeExtension
{
    public static IEnumerable<TreeNode<TKey, TValue>> GetEnds<TKey, TValue>(
        this Tree<TKey, TValue> tree
    )
        where TKey : notnull
    {
        return tree.Root.GetEnds();
    }
}

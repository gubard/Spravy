using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class TreeNodeExtension
{
    public static IEnumerable<TreeNode<TKey, TValue>> GetEnds<TKey, TValue>(
        this TreeNode<TKey, TValue> root
    ) where TKey : notnull
    {
        if (root.Nodes.IsEmpty())
        {
            yield return root;
            yield break;
        }

        foreach (var node in root.Nodes)
        foreach (var end in node.GetEnds())
        {
            yield return end;
        }
    }
}
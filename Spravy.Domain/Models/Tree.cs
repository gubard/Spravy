using Spravy.Domain.Extensions;

namespace Spravy.Domain.Models;

public class Tree<TKey, TValue> where TKey : notnull
{
    public Tree(TreeNode<TKey, TValue> root)
    {
        Root = root.ThrowIfNull();
    }

    public TreeNode<TKey, TValue> this[TKey key]
    {
        get
        {
            if (Root.Key.Equals(key))
            {
                return Root;
            }

            throw new ($"Expected root key {key}.");
        }
    }

    public TreeNode<TKey, TValue> this[params TKey[] keys]
    {
        get
        {
            if (keys.Length == 0)
            {
                return Root;
            }

            var currentNode = this[keys[0]];

            foreach (var key in keys[1..])
            {
                currentNode = currentNode[key];
            }

            return currentNode;
        }
    }

    public TreeNode<TKey, TValue> Root { get; }

    public bool Contains(TKey[] keys)
    {
        if (keys.IsEmpty())
        {
            return false;
        }

        if (!Root.Key.Equals(keys[0]))
        {
            return false;
        }

        if (keys.Length == 1)
        {
            return true;
        }

        var currentNode = this[keys[0]];

        foreach (var key in keys[1..])
        {
            if (!currentNode.NodeKeys.Contains(key))
            {
                return false;
            }

            currentNode = currentNode[key];
        }

        return true;
    }
}
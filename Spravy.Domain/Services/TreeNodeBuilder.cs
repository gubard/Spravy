using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class TreeNodeBuilder<TKey, TValue> : IBuilder<TreeNode<TKey, TValue>> where TKey : notnull
{
    private readonly Dictionary<TKey, TreeNodeBuilder<TKey, TValue?>?> nodes;

    public TreeNodeBuilder()
    {
        nodes = new ();
    }

    public TreeNodeBuilder<TKey, TValue?>? this[TKey key]
    {
        get
        {
            if (nodes.ContainsKey(key))
            {
                return nodes[key];
            }

            var node = new TreeNodeBuilder<TKey, TValue?>
            {
                Key = key
            };

            nodes[key] = node;

            return node;
        }
        set
        {
            nodes[key] = value;
            var node = nodes[key];

            if (node is null)
            {
                return;
            }

            node.Parent = this!;
            node.Key = key;
        }
    }

    public TreeNodeBuilder<TKey, TValue?>? this[TKey key, TValue defaultValue]
    {
        get
        {
            if (nodes.ContainsKey(key))
            {
                return nodes[key];
            }

            var node = new TreeNodeBuilder<TKey, TValue?>
            {
                Key = key,
                Value = defaultValue
            };

            nodes[key] = node;

            return node;
        }
    }

    public TreeNodeBuilder<TKey, TValue?>? this[TValue defaultValue, params TKey[] keys]
    {
        get
        {
            TreeNodeBuilder<TKey, TValue?>? currentNode = this!;

            foreach (var key in keys)
            {
                currentNode = currentNode?[key, defaultValue];
            }

            return currentNode;
        }
        set
        {
            TreeNodeBuilder<TKey, TValue?>? currentNode = this!;

            foreach (var key in keys[..^1])
            {
                currentNode = currentNode?[key, defaultValue];
            }

            if (currentNode is null)
            {
                return;
            }

            currentNode[keys[^1]] = value;
            var node = currentNode[keys[^1]];

            if (node is null)
            {
                return;
            }

            node.Parent = this!;
            node.Key = keys[^1];
        }
    }

    public TreeNodeBuilder<TKey, TValue?>? this[params TKey[] keys]
    {
        get
        {
            TreeNodeBuilder<TKey, TValue?>? currentNode = this!;

            foreach (var key in keys)
            {
                currentNode = currentNode?[key];
            }

            return currentNode;
        }
        set
        {
            TreeNodeBuilder<TKey, TValue?>? currentNode = this!;

            foreach (var key in keys[..^1])
            {
                currentNode = currentNode?[key];
            }

            if (currentNode is null)
            {
                return;
            }

            currentNode[keys[^1]] = value;
            var node = currentNode[keys[^1]];

            if (node is null)
            {
                return;
            }

            node.Parent = this!;
            node.Key = keys[^1];
        }
    }

    public TreeNodeBuilder<TKey, TValue?>? Parent { get; set; }
    public TKey? Key { get; set; }
    public TValue? Value { get; set; }

    public TreeNode<TKey, TValue> Build()
    {
        var key = Key.ThrowIfNull();
        var value = Value.ThrowIfNull();
        var newNodes = nodes.Values.Select(x => x.ThrowIfNull().Build());

        return new (key, value, newNodes!);
    }

    public TreeNodeBuilder<TKey, TValue> Add(TreeNodeBuilder<TKey, TValue?> node)
    {
        var key = node.Key.ThrowIfNull();
        nodes.Add(key, node);
        node.Parent = this!;

        return this;
    }

    public TreeNodeBuilder<TKey, TValue> Add(TKey key, TValue value)
    {
        var node = new TreeNodeBuilder<TKey, TValue?>();
        node.Key = key;
        node.Value = value;

        return Add(node);
    }

    public TreeNodeBuilder<TKey, TValue> SetNode(
        TValue defaultValue,
        TreeNodeBuilder<TKey, TValue?> value,
        params TKey[] keys
    )
    {
        this[defaultValue, keys] = value;

        return this;
    }
}
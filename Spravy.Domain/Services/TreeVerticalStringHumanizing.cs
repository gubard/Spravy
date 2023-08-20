using System.Text;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class TreeVerticalStringHumanizing<TKey, TValue> : IHumanizing<Tree<TKey, TValue>, string>
    where TKey : notnull
{
    private readonly IHumanizing<TreeNode<TKey, TValue>, string> humanizingTreeNode;
    private readonly TreeVerticalStringHumanizingOptions options;

    public TreeVerticalStringHumanizing(
        IHumanizing<TreeNode<TKey, TValue>, string> humanizingTreeNode,
        TreeVerticalStringHumanizingOptions options
    )
    {
        this.humanizingTreeNode = humanizingTreeNode.ThrowIfNull();
        this.options = options.ThrowIfNull();
    }

    public string Humanize(Tree<TKey, TValue> input)
    {
        var stringBuilder = new StringBuilder();
        PrintData(input.Root, options.FirstIndent, stringBuilder);

        return stringBuilder.ToString();
    }

    private void PrintData(TreeNode<TKey, TValue> treeNode, int indent, StringBuilder stringBuilder)
    {
        PrintWithIndent(treeNode, indent, stringBuilder);

        foreach (var node in treeNode.Nodes)
        {
            PrintData(node, indent + options.Indent, stringBuilder);
        }
    }

    private void PrintWithIndent(
        TreeNode<TKey, TValue> treeNode,
        int indent,
        StringBuilder stringBuilder
    )
    {
        var humanizeTreeNode = humanizingTreeNode.Humanize(treeNode);
        var line = string.Format(options.Format, new string(' ', indent * 2), humanizeTreeNode);
        stringBuilder.AppendLine(line);
    }
}
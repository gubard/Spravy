using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class TreeNodeStringHumanizing<TKey, TValue> : IHumanizing<TreeNode<TKey, TValue>, string>
    where TKey : notnull
    where TValue : notnull
{
    private readonly IHumanizing<TKey, string> humanizingKey;
    private readonly IHumanizing<TValue, string> humanizingValue;
    private readonly TreeNodeStringHumanizingOptions options;

    public TreeNodeStringHumanizing(
        TreeNodeStringHumanizingOptions options,
        IHumanizing<TKey, string> humanizingKey,
        IHumanizing<TValue, string> humanizingValue
    )
    {
        this.options = options.ThrowIfNull();
        this.humanizingKey = humanizingKey.ThrowIfNull();
        this.humanizingValue = humanizingValue.ThrowIfNull();
    }

    public string Humanize(TreeNode<TKey, TValue> input)
    {
        var keyHumanize = humanizingKey.Humanize(input.Key);
        var valueHumanize = humanizingValue.Humanize(input.Value);

        return string.Format(options.Fromat, keyHumanize, valueHumanize);
    }
}
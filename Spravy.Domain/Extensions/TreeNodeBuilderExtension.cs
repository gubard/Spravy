using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;

namespace Spravy.Domain.Extensions;

public static class TreeNodeBuilderExtension
{
    public static TreeNodeBuilder<Guid, IModule> Add(
        this TreeNodeBuilder<Guid, IModule> builder,
        IModule module
    )
    {
        return builder.Add(module.Id, module);
    }
    
    public static TreeNodeBuilder<TKey, TValue> SetKey<TKey, TValue>(
        this TreeNodeBuilder<TKey, TValue> builder,
        TKey key
    ) where TKey : notnull
    {
        builder.Key = key;

        return builder;
    }

    public static TreeNodeBuilder<TKey, TValue> SetValue<TKey, TValue>(
        this TreeNodeBuilder<TKey, TValue> builder,
        TValue value
    ) where TKey : notnull
    {
        builder.Value = value;

        return builder;
    }
}
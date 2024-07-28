namespace Spravy.Ui.Extensions;

public static class HostExtension
{
    public static void AddAdaptiveStyle<T>(
        this T host,
        ReadOnlyMemory<MaterialDesignSizeType> types,
        string resourceKey
    )
        where T : IStyleHost, IResourceHost, IDataTemplateHost
    {
        if (!host.TryFindResource(resourceKey, out var resource))
        {
            return;
        }

        if (resource is not IStyle style)
        {
            return;
        }

        var action = (MaterialDesignSizeType type) =>
        {
            if (types.Span.Contains(type))
            {
                if (!host.Styles.Contains(style))
                {
                    host.Styles.Add(style);
                }
            }
            else
            {
                host.Styles.Remove(style);
            }
        };

        MaterialDesignSize.MaterialDesignSizeTypeChanged += action;
        action.Invoke(MaterialDesignSize.LastType);
    }
}

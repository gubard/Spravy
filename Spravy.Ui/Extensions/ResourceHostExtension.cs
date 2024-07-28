namespace Spravy.Ui.Extensions;

public static class ResourceHostExtension
{
    public static bool TryGetStyle<T>(
        this T resourceHost,
        string resourceKey,
        [MaybeNullWhen(false)] out IStyle style
    )
        where T : IStyleHost, IResourceHost
    {
        style = null;

        if (!resourceHost.TryFindResource(resourceKey, out var resource))
        {
            return false;
        }

        if (resource is not IStyle result)
        {
            return false;
        }

        style = result;

        return true;
    }

    public static bool TryGetDataTemplate<T>(
        this T resourceHost,
        string resourceKey,
        [MaybeNullWhen(false)] out IDataTemplate dataTemplate
    )
        where T : IStyleHost, IResourceHost
    {
        dataTemplate = null;

        if (!resourceHost.TryFindResource(resourceKey, out var resource))
        {
            return false;
        }

        if (resource is not IDataTemplate result)
        {
            return false;
        }

        dataTemplate = result;

        return true;
    }
}

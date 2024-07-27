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
}

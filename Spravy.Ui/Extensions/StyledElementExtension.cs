using Avalonia;

namespace Spravy.Ui.Extensions;

public static class StyledElementExtension
{
    public static T? FindParent<T>(this StyledElement element)
    {
        if (element.Parent is null)
        {
            return default;
        }

        if (element.Parent is T result)
        {
            return result;
        }

        return element.Parent.FindParent<T>();
    }
}
namespace Spravy.Ui.Extensions;

public static class VisualExtension
{
    public static T? FindVisualParent<T>(this Visual visual) where T : class
    {
        var visualParent = visual.GetVisualParent();

        if (visualParent is null)
        {
            return default;
        }

        if (visualParent is T result)
        {
            return result;
        }

        return visualParent.FindVisualParent<T>();
    }

    public static T? FindVisualParent<T>(this Visual visual, string name) where T : class
    {
        var visualParent = visual.GetVisualParent();

        if (visualParent is null)
        {
            return default;
        }

        if (visualParent.Name != name)
        {
            return visualParent.FindVisualParent<T>(name);
        }

        if (visualParent is T result)
        {
            return result;
        }

        return visualParent.FindVisualParent<T>(name);
    }
}
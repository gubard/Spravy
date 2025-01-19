namespace Spravy.Integration.Tests.Extensions;

public static class VisualExtension
{
    public static TVisual ClickOn<TVisual>(this TVisual visual, Window window) where TVisual : Visual
    {
        Assert.Equals(visual.IsVisible, true);

        if (visual is InputElement inputElement)
        {
            inputElement.MustEnabled();
        }

        var position = visual.TranslatePoint(new(0, 0), window).ThrowIfNullStruct().Center(visual.Bounds);
        window.RunJobsAll();
        window.SpravyMouseMove(position, RawInputModifiers.None);
        window.RunJobsAll();
        window.SpravyMouseDown(position, MouseButton.Left, RawInputModifiers.None);
        window.RunJobsAll();
        window.SpravyMouseUp(position, MouseButton.Left, RawInputModifiers.None);
        window.RunJobsAll();

        return visual;
    }

    public static TVisual MustWidth<TVisual>(this TVisual visual, double width) where TVisual : Visual
    {
        Assert.Equals(visual.Bounds.Width, width);

        return visual;
    }
}
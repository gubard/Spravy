using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using FluentAssertions;
using Spravy.Domain.Extensions;

namespace Spravy.Tests.Extensions;

public static class VisualExtension
{
    public static TVisual ClickOn<TVisual>(this TVisual button, Window window) where TVisual : Visual
    {
        var position = button.TranslatePoint(new Point(0, 0), window).ThrowIfNullStruct().Center(button.Bounds);
        window.RunJobsAll();
        window.SpravyMouseMove(position, RawInputModifiers.None);
        window.RunJobsAll();
        window.SpravyMouseDown(position, MouseButton.Left, RawInputModifiers.None);
        window.RunJobsAll();
        window.SpravyMouseUp(position, MouseButton.Left, RawInputModifiers.None);
        window.RunJobsAll();

        return button;
    }
    
    public static TVisual MustWidth<TVisual>(this TVisual visual, double width) where TVisual : Visual
    {
        visual.Bounds.Width.Should().Be(width);

        return visual;
    }
}
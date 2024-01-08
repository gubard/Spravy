using Avalonia;
using FluentAssertions;

namespace Spravy.Tests.Extensions;

public static class VisualExtension
{
    public static TVisual MustWidth<TVisual>(this TVisual visual, double width) where TVisual : Visual
    {
        visual.Bounds.Width.Should().Be(width);

        return visual;
    }
}
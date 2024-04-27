using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Spravy.Domain.Extensions;

namespace Spravy.Tests.Extensions;

public static class ButtonExtension
{
    public static Button ClickOnButton(this Button button, Window window)
    {
        var position = button.TranslatePoint(new Point(0, 0), window).ThrowIfNullStruct().Center(button.Bounds);
        Console.WriteLine($"X:{position.X};Y:{position.Y}");
        window.RunJobsAll(1);
        window.SpravyMouseMove(position, RawInputModifiers.None);
        window.RunJobsAll(1);
        window.SpravyMouseDown(position, MouseButton.Left, RawInputModifiers.None);
        window.RunJobsAll(1);
        window.SpravyMouseUp(position, MouseButton.Left, RawInputModifiers.None);
        window.RunJobsAll(1);

        return button;
    }
}
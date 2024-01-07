using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Spravy.Domain.Extensions;

namespace Spravy.Tests.Extensions;

public static class ButtonExtension
{
    public static Button ClickOnButton(this Button button, Window window)
    {
        var position = button.TranslatePoint(new Point(0, 0), window).ThrowIfNullStruct().Center(button.Bounds);
        window.MouseMove(position);
        window.RunJobsAll();
        window.MouseDown(position, MouseButton.Left);
        window.RunJobsAll();
        window.MouseUp(position, MouseButton.Left);
        window.RunJobsAll();

        return button;
    }
}
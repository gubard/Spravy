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

    public static Button ClickOnButton(this Button button, Window window, byte seconds)
    {
        return button.ClickOnButton(window).WaitSeconds(seconds);
    }

    public static Button ClickOnButton(this Button button, Window window, byte seconds, Action predicate)
    {
        return button.ClickOnButton(window).WaitSeconds(seconds, predicate);
    }

    public static T ClickOnButton<T>(this Button button, Window window, byte seconds, Func<T> predicate)
    {
        return button.ClickOnButton(window).WaitSeconds(seconds, predicate);
    }

    public static Task<T> ClickOnButtonAsync<T>(this Button button, Window window, byte seconds, Func<T> predicate)
    {
        return button.ClickOnButton(window).WaitSecondsAsync(seconds, predicate);
    }
}
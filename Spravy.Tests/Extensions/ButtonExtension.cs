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
        window.SpravyMouseMove(position, RawInputModifiers.None);
        window.RunJobsAll();
        window.SpravyMouseDown(position, MouseButton.Left, RawInputModifiers.None);
        window.RunJobsAll();
        window.SpravyMouseUp(position, MouseButton.Left, RawInputModifiers.None);
        window.RunJobsAll();

        return button;
    }

    public static Button ClickOnButton(
        this Button button,
        Window window,
        byte seconds,
        Action predicate
    )
    {
        return button.ClickOnButton(window).WaitSeconds(seconds, predicate);
    }

    public static T ClickOnButton<T>(
        this Button button,
        Window window,
        byte seconds,
        Func<T> predicate
    )
    {
        return button.ClickOnButton(window).WaitSeconds(seconds, predicate);
    }

    public static Task<T> ClickOnButtonAsync<T>(
        this Button button,
        Window window,
        byte seconds,
        Func<T> predicate
    )
    {
        return button.ClickOnButton(window).WaitSecondsAsync(seconds, predicate);
    }
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Spravy.Domain.Extensions;
using Xunit.Abstractions;

namespace Spravy.Tests.Extensions;

public static class ButtonExtension
{
    public static Button ClickOnButton(this Button button, Window window, ITestOutputHelper output)
    {
        output.WriteLine($"Clicking on {button.Name}");
        var position = button.TranslatePoint(new Point(0, 0), window).ThrowIfNullStruct().Center(button.Bounds);
        window.MouseMove(position, RawInputModifiers.None, output);
        window.RunJobsAll(output);
        window.MouseDown(position, MouseButton.Left, RawInputModifiers.None, output);
        window.RunJobsAll(output);
        window.MouseUp(position, MouseButton.Left, RawInputModifiers.None, output);
        window.RunJobsAll(output);
        output.WriteLine($"Clicked on {button.Name}");

        return button;
    }

    public static Button ClickOnButton(this Button button, Window window, byte seconds, ITestOutputHelper output)
    {
        return button.ClickOnButton(window, output).WaitSeconds(seconds, output);
    }

    public static Button ClickOnButton(
        this Button button,
        Window window,
        byte seconds,
        Action predicate,
        ITestOutputHelper output
    )
    {
        return button.ClickOnButton(window, output).WaitSeconds(seconds, predicate, output);
    }

    public static T ClickOnButton<T>(
        this Button button,
        Window window,
        byte seconds,
        Func<T> predicate,
        ITestOutputHelper output
    )
    {
        return button.ClickOnButton(window, output).WaitSeconds(seconds, predicate, output);
    }

    public static Task<T> ClickOnButtonAsync<T>(
        this Button button,
        Window window,
        byte seconds,
        Func<T> predicate,
        ITestOutputHelper output
    )
    {
        return button.ClickOnButton(window, output).WaitSecondsAsync(seconds, predicate, output);
    }
}
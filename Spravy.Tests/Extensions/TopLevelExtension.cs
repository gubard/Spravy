using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Xunit.Abstractions;
using Avalonia.Headless;

namespace Spravy.Tests.Extensions;

public static class TopLevelExtension
{
    public static TopLevel KeyTextInput(
        this TopLevel topLevel,
        string text,
        ITestOutputHelper output
    )
    {
        output.WriteLine($"Key text inputting {text}");
        topLevel.KeyTextInput(text);
        output.WriteLine($"Key text inputted {text}");

        return topLevel;
    }

    public static TopLevel KeyReleaseQwerty(
        this TopLevel topLevel,
        PhysicalKey physicalKey,
        RawInputModifiers modifiers,
        ITestOutputHelper output
    )
    {
        output.WriteLine($"Key releasing qwerty {physicalKey} {modifiers}");
        topLevel.KeyReleaseQwerty(physicalKey, modifiers);
        output.WriteLine($"Key released qwerty {physicalKey} {modifiers}");

        return topLevel;
    }

    public static TopLevel KeyPressQwerty(
        this TopLevel topLevel,
        PhysicalKey physicalKey,
        RawInputModifiers modifiers,
        ITestOutputHelper output
    )
    {
        output.WriteLine($"Key pressing qwerty {physicalKey} {modifiers}");
        topLevel.KeyPressQwerty(physicalKey, modifiers);
        output.WriteLine($"Key pressed qwerty {physicalKey} {modifiers}");

        return topLevel;
    }

    public static TopLevel MouseMove(
        this TopLevel topLevel,
        Point point,
        RawInputModifiers modifiers,
        ITestOutputHelper output
    )
    {
        output.WriteLine($"Mouse moving {point.X}x{point.Y} {modifiers}");
        topLevel.MouseMove(point, modifiers);
        output.WriteLine($"Mouse moved {point.X}x{point.Y} {modifiers}");

        return topLevel;
    }

    public static TopLevel MouseDown(
        this TopLevel topLevel,
        Point point,
        MouseButton button,
        RawInputModifiers modifiers,
        ITestOutputHelper output
    )
    {
        output.WriteLine($"Mouse downing {point.X}x{point.Y} {button} {modifiers}");
        topLevel.MouseDown(point, button, modifiers);
        output.WriteLine($"Mouse downed {point.X}x{point.Y} {button} {modifiers}");

        return topLevel;
    }

    public static TopLevel MouseUp(
        this TopLevel topLevel,
        Point point,
        MouseButton button,
        RawInputModifiers modifiers,
        ITestOutputHelper output
    )
    {
        output.WriteLine($"Mouse upping {point.X}x{point.Y} {button} {modifiers}");
        topLevel.MouseUp(point, button, modifiers);
        output.WriteLine($"Mouse upped {point.X}x{point.Y} {button} {modifiers}");

        return topLevel;
    }
}
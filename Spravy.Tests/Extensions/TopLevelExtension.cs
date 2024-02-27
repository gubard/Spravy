using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Headless;

namespace Spravy.Tests.Extensions;

public static class TopLevelExtension
{
    public static TopLevel SpravyKeyTextInput(
        this TopLevel topLevel,
        string text
    )
    {
        Console.WriteLine($"Key text inputting {text}");
        topLevel.KeyTextInput(text);
        Console.WriteLine($"Key text inputted {text}");

        return topLevel;
    }

    public static TopLevel SpravyKeyReleaseQwerty(
        this TopLevel topLevel,
        PhysicalKey physicalKey,
        RawInputModifiers modifiers
    )
    {
        Console.WriteLine($"Key releasing qwerty {physicalKey} {modifiers}");
        topLevel.KeyReleaseQwerty(physicalKey, modifiers);
        Console.WriteLine($"Key released qwerty {physicalKey} {modifiers}");

        return topLevel;
    }

    public static TopLevel SpravyKeyPressQwerty(
        this TopLevel topLevel,
        PhysicalKey physicalKey,
        RawInputModifiers modifiers
    )
    {
        Console.WriteLine($"Key pressing qwerty {physicalKey} {modifiers}");
        topLevel.KeyPressQwerty(physicalKey, modifiers);
        Console.WriteLine($"Key pressed qwerty {physicalKey} {modifiers}");

        return topLevel;
    }

    public static TopLevel SpravyMouseMove(
        this TopLevel topLevel,
        Point point,
        RawInputModifiers modifiers
    )
    {
        Console.WriteLine($"Mouse moving {point.X}x{point.Y} {modifiers}");
        topLevel.MouseMove(point, modifiers);
        Console.WriteLine($"Mouse moved {point.X}x{point.Y} {modifiers}");

        return topLevel;
    }

    public static TopLevel SpravyMouseDown(
        this TopLevel topLevel,
        Point point,
        MouseButton button,
        RawInputModifiers modifiers
    )
    {
        Console.WriteLine($"Mouse downing {point.X}x{point.Y} {button} {modifiers}");
        topLevel.MouseDown(point, button, modifiers);
        Console.WriteLine($"Mouse downed {point.X}x{point.Y} {button} {modifiers}");

        return topLevel;
    }

    public static TopLevel SpravyMouseUp(
        this TopLevel topLevel,
        Point point,
        MouseButton button,
        RawInputModifiers modifiers
    )
    {
        Console.WriteLine($"Mouse upping {point.X}x{point.Y} {button} {modifiers}");
        topLevel.MouseUp(point, button, modifiers);
        Console.WriteLine($"Mouse upped {point.X}x{point.Y} {button} {modifiers}");

        return topLevel;
    }
}
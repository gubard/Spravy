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
        topLevel.KeyTextInput(text);

        return topLevel;
    }

    public static TopLevel SpravyKeyReleaseQwerty(
        this TopLevel topLevel,
        PhysicalKey physicalKey,
        RawInputModifiers modifiers
    )
    {
        topLevel.KeyReleaseQwerty(physicalKey, modifiers);

        return topLevel;
    }

    public static TopLevel SpravyKeyPressQwerty(
        this TopLevel topLevel,
        PhysicalKey physicalKey,
        RawInputModifiers modifiers
    )
    {
        topLevel.KeyPressQwerty(physicalKey, modifiers);

        return topLevel;
    }

    public static TopLevel SpravyMouseMove(
        this TopLevel topLevel,
        Point point,
        RawInputModifiers modifiers
    )
    {
        topLevel.MouseMove(point, modifiers);

        return topLevel;
    }

    public static TopLevel SpravyMouseDown(
        this TopLevel topLevel,
        Point point,
        MouseButton button,
        RawInputModifiers modifiers
    )
    {
        topLevel.MouseDown(point, button, modifiers);

        return topLevel;
    }

    public static TopLevel SpravyMouseUp(
        this TopLevel topLevel,
        Point point,
        MouseButton button,
        RawInputModifiers modifiers
    )
    {
        topLevel.MouseUp(point, button, modifiers);

        return topLevel;
    }
}
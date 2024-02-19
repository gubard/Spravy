using Avalonia.Input;
using FluentAssertions;
using Xunit.Abstractions;

namespace Spravy.Tests.Extensions;

public static class InputElementExtension
{
    public static TInputElement Focus<TInputElement>(
        this TInputElement inputElement,
        NavigationMethod method,
        KeyModifiers keyModifiers,
        ITestOutputHelper output
    )
        where TInputElement : InputElement
    {
        output.WriteLine($"Focusing {inputElement.Name}");
        inputElement.Focus(method, keyModifiers);
        output.WriteLine($"Focused {inputElement.Name}");

        return inputElement;
    }

    public static TInputElement FocusElement<TInputElement>(this TInputElement inputElement, ITestOutputHelper output)
        where TInputElement : InputElement
    {
        inputElement.IsFocused.Should().BeFalse();
        inputElement.Focus(NavigationMethod.Unspecified, KeyModifiers.None, output);
        inputElement.RunJobsAll(output);
        inputElement.IsFocused.Should().BeTrue();

        return inputElement;
    }

    public static TInputElement MustFocused<TInputElement>(this TInputElement inputElement)
        where TInputElement : InputElement
    {
        inputElement.IsFocused.Should().BeTrue();

        return inputElement;
    }

    public static TInputElement MustEnabled<TInputElement>(this TInputElement inputElement)
        where TInputElement : InputElement
    {
        inputElement.IsEnabled.Should().BeTrue();

        return inputElement;
    }
}
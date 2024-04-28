using Avalonia.Input;
using FluentAssertions;

namespace Spravy.Integration.Tests.Extensions;

public static class InputElementExtension
{
    public static TInputElement SpravyFocus<TInputElement>(
        this TInputElement inputElement,
        NavigationMethod method,
        KeyModifiers keyModifiers
    )
        where TInputElement : InputElement
    {
        inputElement.Focus(method, keyModifiers);

        return inputElement;
    }

    public static TInputElement FocusElement<TInputElement>(this TInputElement inputElement)
        where TInputElement : InputElement
    {
        inputElement.IsFocused.Should().BeFalse();
        inputElement.SpravyFocus(NavigationMethod.Unspecified, KeyModifiers.None);
        inputElement.RunJobsAll();
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
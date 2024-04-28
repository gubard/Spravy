using Avalonia.Input;
using FluentAssertions;

namespace Spravy.Integration.Tests.Extensions;

public static class InputElementExtension
{
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
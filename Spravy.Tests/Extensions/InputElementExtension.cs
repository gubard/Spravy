using Avalonia.Input;
using FluentAssertions;

namespace Spravy.Tests.Extensions;

public static class InputElementExtension
{
    public static TInputElement FocusElement<TInputElement>(this TInputElement inputElement)
        where TInputElement : IInputElement
    {
        inputElement.IsFocused.Should().BeFalse();
        inputElement.Focus();
        inputElement.RunJobsAll();
        inputElement.IsFocused.Should().BeTrue();

        return inputElement;
    }
    
    public static TInputElement MustFocused<TInputElement>(this TInputElement inputElement)
        where TInputElement : IInputElement
    {
        inputElement.IsFocused.Should().BeTrue();

        return inputElement;
    }
    
    public static TInputElement MustEnabled<TInputElement>(this TInputElement inputElement)
        where TInputElement : IInputElement
    {
        inputElement.IsEnabled.Should().BeTrue();

        return inputElement;
    }
}
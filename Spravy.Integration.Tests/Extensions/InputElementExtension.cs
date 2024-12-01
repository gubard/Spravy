namespace Spravy.Integration.Tests.Extensions;

public static class InputElementExtension
{
    public static TInputElement MustFocused<TInputElement>(this TInputElement inputElement)
        where TInputElement : InputElement
    {
        inputElement.IsFocused.Should().BeTrue($"{inputElement.Name} must be focused");

        return inputElement;
    }

    public static TInputElement MustEnabled<TInputElement>(this TInputElement inputElement)
        where TInputElement : InputElement
    {
        inputElement.IsEnabled.Should().BeTrue($"{inputElement.Name} must be enabled");

        return inputElement;
    }
}
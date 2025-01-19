namespace Spravy.Integration.Tests.Extensions;

public static class InputElementExtension
{
    public static TInputElement MustFocused<TInputElement>(this TInputElement inputElement)
        where TInputElement : InputElement
    {
        Assert.Equals(inputElement.IsFocused, true);

        return inputElement;
    }

    public static TInputElement MustEnabled<TInputElement>(this TInputElement inputElement)
        where TInputElement : InputElement
    {
        Assert.Equals(inputElement.IsEnabled, true);

        return inputElement;
    }
}
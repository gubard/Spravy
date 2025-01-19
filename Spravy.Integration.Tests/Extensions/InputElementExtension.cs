using Shouldly;

namespace Spravy.Integration.Tests.Extensions;

public static class InputElementExtension
{
    public static TInputElement MustFocused<TInputElement>(this TInputElement inputElement)
        where TInputElement : InputElement
    {
        inputElement.IsFocused.ShouldBe(true);

        return inputElement;
    }

    public static TInputElement MustEnabled<TInputElement>(this TInputElement inputElement)
        where TInputElement : InputElement
    {
        inputElement.IsEnabled.ShouldBe(true);

        return inputElement;
    }
}
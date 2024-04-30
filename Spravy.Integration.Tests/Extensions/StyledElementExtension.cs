using Avalonia;
using FluentAssertions;

namespace Spravy.Integration.Tests.Extensions;

public static class StyledElementExtension
{
    public static TStyleable MustHasError<TStyleable>(this TStyleable styledElement) where TStyleable : StyledElement
    {
        styledElement.Classes.Should().Contain(":error");

        return styledElement;
    }

    public static TStyleable MustNotHasError<TStyleable>(this TStyleable styledElement) where TStyleable : StyledElement
    {
        styledElement.Classes.Should().NotContain(":error");

        return styledElement;
    }
}
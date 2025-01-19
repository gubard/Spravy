using Shouldly;

namespace Spravy.Integration.Tests.Extensions;

public static class StyledElementExtension
{
    public static TStyleable MustHasError<TStyleable>(this TStyleable styledElement) where TStyleable : StyledElement
    {
        styledElement.Classes.ShouldContain(":error");

        return styledElement;
    }

    public static TStyleable MustNotHasError<TStyleable>(this TStyleable styledElement) where TStyleable : StyledElement
    {
        styledElement.Classes.ShouldNotContain(":error");

        return styledElement;
    }
}
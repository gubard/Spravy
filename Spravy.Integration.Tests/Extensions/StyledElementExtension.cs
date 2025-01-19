namespace Spravy.Integration.Tests.Extensions;

public static class StyledElementExtension
{
    public static TStyleable MustHasError<TStyleable>(this TStyleable styledElement) where TStyleable : StyledElement
    {
        Assert.Equals(true, styledElement.Classes.Contains(":error"));

        return styledElement;
    }

    public static TStyleable MustNotHasError<TStyleable>(this TStyleable styledElement) where TStyleable : StyledElement
    {
        Assert.Equals(false, styledElement.Classes.Contains(":error"));

        return styledElement;
    }
}
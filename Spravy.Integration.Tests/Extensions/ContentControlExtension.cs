namespace Spravy.Integration.Tests.Extensions;

public static class ContentControlExtension
{
    public static TView GetContentView<TView>(this ContentControl contentControl)
    {
        return contentControl
            .GetVisualChildren()
            .Single()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<TView>();
    }
}

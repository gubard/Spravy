namespace Spravy.Ui.Services;

public class JetBrainsMonoFontCollection : EmbeddedFontCollection
{
    public JetBrainsMonoFontCollection() : base(
        new("fonts:JetBrainsMono", UriKind.Absolute),
        new("avares://Spravy.Ui/Assets/Fonts/JetBrainsMono", UriKind.Absolute)
    )
    {
    }
}
namespace Spravy.Ui.Services;

public class ShantellSansFontCollection : EmbeddedFontCollection
{
    public ShantellSansFontCollection()
        : base(
            new("fonts:ShantellSans", UriKind.Absolute),
            new("avares://Spravy.Ui/Assets/Fonts/ShantellSans", UriKind.Absolute)
        ) { }
}

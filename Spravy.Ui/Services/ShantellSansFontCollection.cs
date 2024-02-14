using System;
using Avalonia.Media.Fonts;

namespace Spravy.Ui.Services;

public class ShantellSansFontCollection : EmbeddedFontCollection
{
    public ShantellSansFontCollection() : base(
        new Uri("fonts:ShantellSans", UriKind.Absolute),
        new Uri("avares://Spravy.Ui/Assets/Fonts/ShantellSans", UriKind.Absolute)
    )
    {
    }
}
namespace Spravy.Ui.Features.Localizations.Models;

public class HeaderView : TextView
{
    public HeaderView(string key) : base(key)
    {
    }

    public HeaderView(string key, object? parameters) : base(key, parameters)
    {
    }
}
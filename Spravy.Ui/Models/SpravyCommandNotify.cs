namespace Spravy.Ui.Models;

public class SpravyCommandNotify : NotifyBase
{
    public SpravyCommandNotify(MaterialIconKind kind, TextLocalization text, SpravyCommand item)
    {
        Item = item;
        Text = text;
        Kind = kind;
    }

    public SpravyCommand Item { get; }
    public MaterialIconKind Kind { get; }
    public TextLocalization Text { get; }
}
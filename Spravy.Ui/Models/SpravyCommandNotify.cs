using Avalonia.Input;

namespace Spravy.Ui.Models;

public class SpravyCommandNotify : NotifyBase
{
    public SpravyCommandNotify(MaterialIconKind kind, TextLocalization text, SpravyCommand item)
    {
        Item = item;
        Text = text;
        Kind = kind;
    }

    public SpravyCommandNotify(
        MaterialIconKind kind,
        TextLocalization text,
        SpravyCommand item,
        KeyGesture hotKey
    )
    {
        Item = item;
        Text = text;
        Kind = kind;
        HotKey = hotKey;
    }

    public SpravyCommand Item { get; }
    public MaterialIconKind Kind { get; }
    public TextLocalization Text { get; }
    public KeyGesture? HotKey { get; }
}

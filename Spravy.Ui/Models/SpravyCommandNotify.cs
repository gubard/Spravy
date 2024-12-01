namespace Spravy.Ui.Models;

public class SpravyCommandNotify : NotifyBase
{
    public SpravyCommandNotify(string kind, TextLocalization text, SpravyCommand item)
    {
        Item = item;
        Text = text;
        Kind = kind;
    }

    public SpravyCommandNotify(string kind, TextLocalization text, SpravyCommand item, KeyGesture hotKey)
    {
        Item = item;
        Text = text;
        Kind = kind;
        HotKey = hotKey;
    }

    public SpravyCommand Item { get; }
    public string Kind { get; }
    public TextLocalization Text { get; }
    public KeyGesture? HotKey { get; }
}
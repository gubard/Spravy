namespace Spravy.Ui.Models;

public class SpravyCommandNotify : NotifyBase
{
    public SpravyCommandNotify(SpravyCommand item, MaterialIconKind kind)
    {
        Item = item;
        Kind = kind;
    }
    
    public SpravyCommand Item { get; }
    public MaterialIconKind Kind { get; }
}
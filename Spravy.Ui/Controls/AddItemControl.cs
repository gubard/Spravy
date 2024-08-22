namespace Spravy.Ui.Controls;

public abstract class AddItemControl : Control, IAddItem
{
    public abstract object Value { get; }
}

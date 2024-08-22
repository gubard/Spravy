namespace Spravy.Ui.Controls;

public abstract class AddItemControl : TemplatedControl, IAddItem
{
    public abstract object Value { get; }
}

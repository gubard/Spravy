namespace Spravy.Ui.Models;

public class CheckedItem<TItem> : NotifyBase
{
    private bool isChecked;
    private TItem? item;

    public bool IsChecked
    {
        get => isChecked;
        set => this.RaiseAndSetIfChanged(ref isChecked, value);
    }

    public TItem? Item
    {
        get => item;
        set => this.RaiseAndSetIfChanged(ref item, value);
    }
}
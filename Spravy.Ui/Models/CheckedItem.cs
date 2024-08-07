namespace Spravy.Ui.Models;

public partial class CheckedItem<TItem> : NotifyBase
{
    [ObservableProperty]
    private bool isChecked;

    [ObservableProperty]
    private TItem? item;
}

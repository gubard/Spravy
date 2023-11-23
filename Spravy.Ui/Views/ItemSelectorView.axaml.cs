using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ItemSelectorView : ReactiveUserControl<ItemSelectorViewModel>
{
    public ItemSelectorView()
    {
        InitializeComponent();
    }
}
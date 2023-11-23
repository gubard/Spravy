using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class LeafToDoItemsView : ReactiveUserControl<LeafToDoItemsViewModel>
{
    public LeafToDoItemsView()
    {
        InitializeComponent();
    }
}
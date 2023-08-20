using Avalonia.Markup.Xaml;
using Spravy.Ui.Controls;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class LeafToDoItemsView : MainReactiveUserControl<LeafToDoItemsViewModel>
{
    public LeafToDoItemsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
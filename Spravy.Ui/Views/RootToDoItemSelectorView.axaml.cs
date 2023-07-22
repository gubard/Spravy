using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class RootToDoItemSelectorView : ReactiveUserControl<RootToDoItemSelectorViewModel>
{
    public RootToDoItemSelectorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
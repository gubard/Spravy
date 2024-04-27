using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class RootToDoItemsView : ReactiveUserControl<RootToDoItemsViewModel>
{
    public const string AddRootToDoItemButtonName = "add-root-to-do-item-button";
    
    public RootToDoItemsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
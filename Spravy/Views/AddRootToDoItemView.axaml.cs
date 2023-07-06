using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Spravy.ViewModels;

namespace Spravy.Views;

public partial class AddRootToDoItemView : ReactiveUserControl<AddRootToDoItemViewModel>
{
    public AddRootToDoItemView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
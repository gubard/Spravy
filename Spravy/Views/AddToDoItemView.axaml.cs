using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Spravy.ViewModels;

namespace Spravy.Views;

public partial class AddToDoItemView : ReactiveUserControl<AddToDoItemViewModel>
{
    public AddToDoItemView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}